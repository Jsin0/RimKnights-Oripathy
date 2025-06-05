using RimWorld;
using System;
using Verse;

namespace RimKnights
{
    [DefOf]
    public static class EffecterDefOf
    {
        static EffecterDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(EffecterDefOf));
        }

        public static EffecterDef RK_ShatterWarmup;
        public static EffecterDef RK_Shattering;

    }
}
