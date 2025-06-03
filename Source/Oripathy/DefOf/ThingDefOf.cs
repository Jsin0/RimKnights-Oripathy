using RimWorld;
using System;
using Verse;

namespace RimKnights
{
    [DefOf]
    public static class ThingDefOf
    {
        static ThingDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(ThingDefOf));
        }

        public static ThingDef RK_OriginiumCluster;
    }
}
