using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;

namespace RimKnights.Oripathy
{
    [StaticConstructorOnStartup]
    internal static class ApplyHarmony
    {
        static ApplyHarmony()
        {
            Log.Message("DebugOripathyLoaded".Translate());
            Harmony harmony = new Harmony("Jsin.RK_Oripathy.Harmony");
            harmony.PatchAll();
            if (OripathyMod.settings.debugMode) Log.Message("DebugOripathyPatchesLoaded".Translate());
        }
    }
}
