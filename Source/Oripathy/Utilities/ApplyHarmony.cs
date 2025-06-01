using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;

namespace Originium.Utilities
{
    [StaticConstructorOnStartup]
    internal static class ApplyHarmony
    {
        static ApplyHarmony()
        {

            Log.Message("RK_Oripathy loaded.");
            Harmony harmony = new Harmony("Jsin.RK_Oripathy.Harmony");
            harmony.PatchAll();
            Log.Message("RK_Oripathy patches applied");
        }
        [HarmonyPatch(typeof(Corpse))]
        [HarmonyPatch("TickRare", 0)]
        static class Corpse_TickRare_Patch
        {
            [HarmonyPostfix]
            static void PostFix(Corpse __instance)
            {
                if (__instance == null)
                {
                    //Log.Error("corpse no longer exists");
                    return;
                }
                Hediff_Oripathy firstHediff = __instance.InnerPawn.health.hediffSet.GetFirstHediff<Hediff_Oripathy>();
                if (firstHediff != null)
                {
                    firstHediff.TickRare();
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
                if (!pawn.Spawned)
                {
                    //Log.Error("corpse no longer exists");
                    return;
                }
                if (pawn.IsHashIntervalTick(3451) && pawn.Position.IsPolluted(pawn.Map))
                {
                    GameCondition_OriginiumRain.DoPawnOriDamage(pawn, false, false);
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
               __result = __result || surgeryTarget.health.hediffSet.HasHediff(HediffDefOf.RK_OriginiumBuildup);
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
                __result = __result || pawn.health.hediffSet.HasHediff(HediffDefOf.RK_OriginiumBuildup);
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
                Hediff firstHediffOfDef = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.RK_OriginiumBuildup);
                if(firstHediffOfDef != null)
                {
                    foreach (Thing thing in ingredients)
                    {
                        Log.Message($"Ingredient: {thing.def.defName}, stackCount: {thing.stackCount}, IsMedicine: {thing.def.IsMedicine}");
                    }
                    float num = 0f ;
                    for (int i = 0; i < ingredients.Count; i++)
                    {
                        if (!ingredients[i].def.IsMedicine)
                        {
                            Log.Message(ingredients[i].stackCount);
                            num += 0.20f * (float)ingredients[i].stackCount;
                        }
                    }
                    Log.Message(num);
                    if(num > 0f)
                    {
                        firstHediffOfDef.Severity -= num;
                    }
                }
            }
        }
    }
}
