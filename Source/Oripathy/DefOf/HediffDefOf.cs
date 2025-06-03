using RimWorld;
using System;
using Verse;

namespace Originium
{
    [DefOf]
    public static class HediffDefOf
    {
        static HediffDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(HediffDefOf));
        }
        public static HediffDef RK_Oripathy;

        public static HediffDef RK_OriginiumBuildup;

        public static HediffDef RK_HarvestShock;
    }
}
