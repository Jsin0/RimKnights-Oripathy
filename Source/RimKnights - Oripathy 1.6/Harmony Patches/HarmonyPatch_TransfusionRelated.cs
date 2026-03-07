using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using RimWorld;
using Verse;

namespace RimKnights.Oripathy
{
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
            __result = __result || (pawn?.health?.hediffSet.GetFirstHediffOfDef(HediffDefOf.RK_OriginiumBuildup)?.Severity ?? 0f) > 0.01f;
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
            if (firstHediffOfDef != null)
            {
                float num = 0f;
                for (int i = 0; i < ingredients.Count; i++)
                {
                    if (!ingredients[i].def.IsMedicine)
                    {
                        num += 0.20f * (float)ingredients[i].stackCount;
                    }
                }
                if (num > 0f)
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


}
