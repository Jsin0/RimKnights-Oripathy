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
    [HarmonyPatch(typeof(Corpse))]
    [HarmonyPatch("TickRare", 0)]
    static class Corpse_TickRare_Patch
    {
        [HarmonyPostfix]
        public static void PostFix(Corpse __instance)
        {
            //Log.Message("PostFix Start");
            if (__instance.DestroyedOrNull() || __instance.InnerPawn == null || __instance.InnerPawn.health == null || !__instance.InnerPawn.Dead) return;

            Hediff_Oripathy hediff = __instance.InnerPawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.RK_Oripathy) as Hediff_Oripathy;
            if (hediff != null)
            {
                //Log.Message("Ticking hediff");
                hediff.TickRare();
            }
        }
    }

    //Graves and Sarcophagi no longer tick corpses as of 1.6
    [HarmonyPatch(typeof(ThingWithComps), "TickRare")]
    static class ThingTickRarePatch
    {
        [HarmonyPostfix]
        static void Postfix(ThingWithComps __instance)
        {
            if (__instance is Building_CorpseCasket grave && grave.HasCorpse)
            {
                Pawn innerPawn = grave.Corpse.InnerPawn;
                Hediff_Oripathy hediff = innerPawn?.health?.hediffSet?.GetFirstHediffOfDef(HediffDefOf.RK_Oripathy) as Hediff_Oripathy;
                hediff?.TickRare();
            }
        }
    }

}
