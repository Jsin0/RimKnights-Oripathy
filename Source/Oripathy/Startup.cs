using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using HarmonyLib;

namespace Originium.Utilities
{
    [StaticConstructorOnStartup]
    internal static class Startup
    {
        //static Startup()
        //{
        //}
        static Startup()
        {

            Log.Message("RK_Oripathy loaded.");
            Harmony harmony = new Harmony("Jsin.RK_Oripathy.Harmony");
            harmony.PatchAll();
            Log.Message("RK_Oripathy patches applied");
        }
        [HarmonyPatch(typeof(Corpse))]
        [HarmonyPatch("TickRare",0)]
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
                if(firstHediff != null)
                {
                    firstHediff.TickRare();
                }
            }
        }
    }
}
