using RimWorld;
using System;
using Verse;

namespace Originium
{
    [DefOf]
    public static class StatDefOf
    {
        static StatDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(StatDefOf));
        }

        public static StatDef RK_OriginiumResistance;

    }
}
