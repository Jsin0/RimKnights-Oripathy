using RimWorld;
using System;
using Verse;

namespace RimKnights
{
    public class HediffCompProperties_DynamicSeverityRange : HediffCompProperties
    {
        public HediffCompProperties_DynamicSeverityRange()
        {
            this.compClass = typeof(HediffComp_DynamicSeverityRange);
        }

        public AffectorHediff minAffector;

        public AffectorHediff maxAffector;

        public int updateInterval = 250;
    }
}
