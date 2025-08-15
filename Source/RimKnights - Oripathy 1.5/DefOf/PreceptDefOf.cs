using System;
using RimWorld;

namespace RimKnights.Oripathy
{
    [DefOf]
    public static class PreceptDefOf
    {
        static PreceptDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(PreceptDefOf));
        }

        public static PreceptDef Oripathy_Exalted;

        public static PreceptDef Oripathy_Required;
    }
}
