using RimWorld;
using System;
using Verse;

namespace RimKnights
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

        [MayRequire("rimknights.industry")]
        public static HediffDef RK_HarvestShock;
    }
}
