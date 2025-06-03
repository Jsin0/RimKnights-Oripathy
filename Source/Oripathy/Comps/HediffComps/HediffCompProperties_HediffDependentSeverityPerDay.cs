using System;
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace RimKnights
{
    public class HediffCompProperties_HediffDependentSeverityPerDay : HediffCompProperties
    {
        public HediffCompProperties_HediffDependentSeverityPerDay()
        {
            this.compClass = typeof(HediffComp_HediffDependentSeverityPerDay);
        }

        public List<AffectorHediff> AffectorHediffs;

        public float mechanitorFactor = 1f;

        public float minAge = 0f;

    }
}
