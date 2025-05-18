using HarmonyLib;
using RimWorld;
using System;
using Verse;

namespace Originium.Utilities
{
    [StaticConstructorOnStartup]
    internal static class Startup
    {
        static Startup()
        {

            Log.Message("RK_Oripathy loaded.");
            Harmony harmony = new Harmony("Jsin.RK_Oripathy.Harmony");
            harmony.PatchAll();
            Log.Message("RK_Oripathy patches applied");
        }
        [HarmonyPatch(typeof(Corpse))]
        [HarmonyPatch("TickRare", 0)]
        public class Corpse_TickRare_Patch
        {
            [HarmonyPostfix]
            public static void PostFix(Corpse __instance)
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
        public class PawnPollutionTick_Patch
        {
            [HarmonyPostfix]
            public static void PostFix(Pawn pawn)
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
    }
}
