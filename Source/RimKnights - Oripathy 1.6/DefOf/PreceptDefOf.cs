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

        [MayRequireIdeology]
        public static PreceptDef Oripathy_Exalted;

        [MayRequireIdeology]
        public static PreceptDef Oripathy_Required;
    }
}
