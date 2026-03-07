using RimWorld;
using System;
using Verse;

namespace RimKnights.Oripathy
{
    [DefOf]
    public static class DamageDefOf
    {
        static DamageDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(DamageDefOf));
        }

        public static DamageDef RK_ActiveOriginium;

        public static DamageDef RK_OriginiumBlast;

        public static DamageDef RK_OriginiumCut;

    }
}
