using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using HarmonyLib;

namespace Oripathy.Utilities
{
    [StaticConstructorOnStartup]
    internal static class Startup
    {
        //static Startup()
        //{
        //}
        static Startup()
        {

            Log.Message("Oripathy loaded.");
            Harmony harmony = new Harmony("Jsin.Oripathy.Harmony");
            harmony.PatchAll();
            Log.Message("Oripathy patches applied");
        }
        [HarmonyPatch(typeof(Corpse))]
        [HarmonyPatch("TickRare",0)]
        public class Corpse_TickRare_Patch
        {
            [HarmonyPostfix]
            public static void PostFix(Corpse __instance)
            {
                Hediff_Oripathy firstHediff = __instance.InnerPawn.health.hediffSet.GetFirstHediff<Hediff_Oripathy>();
                if(firstHediff != null)
                {
                    firstHediff.TickRare();
                }
            }
        }
    }
}
