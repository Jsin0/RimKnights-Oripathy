using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;

namespace RimKnights.Oripathy.Utilities
{
    [StaticConstructorOnStartup]
    internal static class ApplyHarmony
    {
        static ApplyHarmony()
        {

            if (Core.CoreMod.settings.debugMode) Log.Message("RimKnights - Oripathy loaded.");
            Harmony harmony = new Harmony("Jsin.RK_Oripathy.Harmony");
            harmony.PatchAll();
            if (Core.CoreMod.settings.debugMode) Log.Message("RK_Oripathy patches applied");
        }
        [HarmonyPatch(typeof(Corpse))]
        [HarmonyPatch("TickRare", 0)]
        static class Corpse_TickRare_Patch
        {
            [HarmonyPostfix]
            public static void PostFix(Corpse __instance)
            {
                if(__instance.DestroyedOrNull() || __instance.InnerPawn == null || __instance.InnerPawn.health == null ||!__instance.InnerPawn.Dead) return;
                
                Hediff_Oripathy hediff = __instance.InnerPawn?.health?.hediffSet.GetFirstHediffOfDef(HediffDefOf.RK_Oripathy) as Hediff_Oripathy;
                if(hediff != null)
                {
                    hediff.TickRare();
                }
            }
        }

        [HarmonyPatch(typeof(PollutionUtility))]
        [HarmonyPatch("PawnPollutionTick")]
        static class PawnPollutionTick_Patch
        {
            [HarmonyPostfix]
            static void PostFix(Pawn pawn)
            {
                if (pawn.Spawned)
                {
                    if (pawn.IsHashIntervalTick(3451) && pawn.Position.IsPolluted(pawn.Map))
                    {
                        Core.GameCondition_OriginiumRain.DoPawnOriDamage(pawn, false, false);
                    }
                }
            }
        }

        //Recipe_BloodTransfusion Patches
        [HarmonyPatch(typeof(Recipe_BloodTransfusion))]
        [HarmonyPatch("CompletableEver")]
        static class CompletableEverPatch
        {
            [HarmonyPostfix]
            static void PostFix(Pawn surgeryTarget, ref bool __result)
            {
               __result = __result || surgeryTarget.health.hediffSet.HasHediff(Core.HediffDefOf.RK_OriginiumBuildup);
            }
        }

        [HarmonyPatch(typeof(Recipe_BloodTransfusion))]
        [HarmonyPatch("AvailableOnNow")]
        static class AvailableOnNowPatch
        {
            [HarmonyPostfix]
            static void PostFix(Thing thing, ref bool __result)
            {
                Pawn pawn = thing as Pawn;
                __result = __result || (pawn.health?.hediffSet.GetFirstHediffOfDef(HediffDefOf.RK_OriginiumBuildup)?.Severity ?? 0f) > 0.01f;
            }
        }


        [HarmonyPatch(typeof(Recipe_BloodTransfusion))]
        [HarmonyPatch("ApplyOnPawn")]
        static class ApplyOnPawnPatch
        {
            [HarmonyPrefix]
            static void PreFix(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
            {
                if (!ModsConfig.BiotechActive) return;
                Hediff firstHediffOfDef = pawn.health.hediffSet.GetFirstHediffOfDef(Core.HediffDefOf.RK_OriginiumBuildup);
                if(firstHediffOfDef != null)
                {
                    float num = 0f ;
                    for (int i = 0; i < ingredients.Count; i++)
                    {
                        if (!ingredients[i].def.IsMedicine)
                        {
                            num += 0.20f * (float)ingredients[i].stackCount;
                        }
                    }
                    if(num > 0f)
                    {
                        firstHediffOfDef.Severity -= num;
                    }
                }
            }
        }

        [HarmonyPatch(typeof(Bill_Medical))]
        [HarmonyPatch("PawnAllowedToStartAnew")]
        static class PawnAllowedToStartAnewPatch
        {
            [HarmonyPostfix]
            static void Postfix(Pawn pawn, Bill_Medical __instance, ref bool __result)
            {
                if (__result && (__instance.recipe == RecipeDefOf.RK_ExciseLesion || __instance.recipe == RecipeDefOf.RK_HarvestLesion) && !new HistoryEvent(HistoryEventDefOf.RK_RemovedLesion, pawn.Named(HistoryEventArgsNames.Doer)).Notify_PawnAboutToDo_Job())
                {
                    __result = false;
                }
            }
        }

        [HarmonyPatch(typeof(PawnGenerator))]
        [HarmonyPatch("AddBlindness")]
        static class AddBlindness_AddOripathy
        {
            [HarmonyPostfix]
            static void Postfix(Pawn pawn)
            {

                if (pawn.ideo == null || pawn.ideo.Ideo == null || pawn.health == null || pawn.health.hediffSet.HasHediff(HediffDefOf.RK_Oripathy))
                {
                    return;
                }
                if (Rand.Chance(pawn.ideo.Ideo.GetOripathicPawnChance()))
                {
                    IEnumerable<BodyPartRecord> partsToApplyOn = JobDriver_Infect.GetPartsToApplyOn(pawn);
                    List<BodyPartRecord> list = partsToApplyOn.ToList();

                    if (list.Count > 0)
                    {
                        JobDriver_Infect.Infect(pawn, list.RandomElement<BodyPartRecord>());
                    }

                    pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.RK_Oripathy).Severity = new FloatRange(0.1f, 0.15f).RandomInRange;
                    pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.RK_OripathyLesion).Severity = new FloatRange(0.1f, 0.25f).RandomInRange;

                }
            }
        }

        [HarmonyPatch(typeof(PawnApparelGenerator))]
        [HarmonyPatch("GenerateStartingApparelFor")]
        static class GenerateStartingApparel_GiveInfectionMonitor
        {
            [HarmonyPostfix]
            static void Postfix(Pawn pawn, PawnGenerationRequest request)
            {
                if (!OripathyMod.infectionMonitor || !pawn.RaceProps.ToolUser || !pawn.RaceProps.IsFlesh || pawn.RaceProps.IsAnomalyEntity || pawn.Faction == null)
                {
                    return;
                }
                if(pawn.health == null || !pawn.health.hediffSet.HasHediff(HediffDefOf.RK_Oripathy))
                {
                    return;
                }

                float chance = 0f;
                switch(pawn.Faction.def.techLevel)
                {
                    case TechLevel.Archotech:
                        chance = 1f;
                        break;
                    case TechLevel.Ultra:
                        chance = 0.8f;
                        break;
                    case TechLevel.Spacer:
                        chance = 0.65f;
                        break;
                    case TechLevel.Industrial:
                        chance = 0.25f;
                        break;
                }

                if (Rand.Chance(chance))
                {
                    if (!Rand.Chance(chance))
                    {
                        Apparel apparel = (Apparel)ThingMaker.MakeThing(ThingDefOf.RK_InfectionMonitor);
                        if (pawn.apparel.CanWearWithoutDroppingAnything(apparel.def))
                        {
                            pawn.apparel.Wear(apparel, false);
                            return;
                        }
                    }

                    BodyPartRecord bodyPartTorso = pawn?.RaceProps?.body?.AllParts.FirstOrDefault(p => p.def == BodyPartDefOf.Torso);
                    if (bodyPartTorso != null && !pawn.health.hediffSet.HasHediff(HediffDefOf.RK_InfectionMonitorImplant))
                    {
                        pawn.health.GetOrAddHediff(HediffDefOf.RK_InfectionMonitorImplant,bodyPartTorso);
                    }
                }


            }
        }
    }
}
