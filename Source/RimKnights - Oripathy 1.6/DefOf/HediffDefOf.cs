using RimWorld;
using System;
using Verse;

namespace RimKnights.Oripathy
{
    [DefOf]
    public static class HediffDefOf
    {
        static HediffDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(HediffDefOf));
        }

        public static HediffDef RK_OripathyLesion; 
        
        public static HediffDef RK_Oripathy;

        public static HediffDef RK_OriginiumBuildup;

        public static HediffDef RK_HarvestShock;

        public static HediffDef RK_InfectionMonitorWorn;

        public static HediffDef RK_InfectionMonitorImplant;
    }
}
