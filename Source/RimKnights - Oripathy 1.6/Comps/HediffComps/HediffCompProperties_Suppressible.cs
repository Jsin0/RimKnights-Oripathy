using RimWorld;
using System;
using Verse;

namespace RimKnights.Oripathy
{
    public class HediffCompProperties_Suppressible : HediffCompProperties
    {
        public HediffCompProperties_Suppressible()
        {
            this.compClass = typeof(HediffComp_Suppressible);
        }

        public HediffDef suppressor;

        public HediffDef suppressedHediff;

        public HediffDef unsuppressedHediff;

        public int checkInterval = 250;
    }
}
