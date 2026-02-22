using System;
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace RimKnights.Oripathy
{
    public class HediffCompProperties_DynamicSeverityPerDay : HediffCompProperties
    {
        public HediffCompProperties_DynamicSeverityPerDay()
        {
            this.compClass = typeof(HediffComp_DynamicSeverityPerDay);
        }

        public List<AffectorHediff> AffectorHediffs;

        public float mechanitorFactor = 1f;

        public float minAge = 0f;

    }
}
