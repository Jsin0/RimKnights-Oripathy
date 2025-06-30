using RimWorld;
using Verse;

namespace RimKnights.Oripathy
{
    [DefOf]
    public static class ThingDefOf
    {
        static ThingDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(ThingDefOf));
        }

        public static ThingDef RK_InfectionMonitor;

        public static ThingDef RK_ShatterGlow;
    }
}
