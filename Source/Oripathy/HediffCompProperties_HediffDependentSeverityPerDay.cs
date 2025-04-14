using RimWorld;
using System;
using Verse;

namespace Originium
{
    public class HediffCompProperties_HediffDependentSeverityPerDay : HediffCompProperties
    {
        public HediffCompProperties_HediffDependentSeverityPerDay()
        {
            this.compClass = typeof(HediffComp_HediffDependentSeverityPerDay);
        }

        public float CalculateSeverityPerDay(float affectorSeverity = 0f)
        {
            float sev;
            if (severityCurve != null)
            {
                sev = this.severityCurve.Evaluate(affectorSeverity);
            }
            else
            {
                sev = (this.severityFactor * affectorSeverity) + this.severityOffset;
            }

            return sev + this.severityPerDayRange.RandomInRange;
        }

        public FloatRange severityPerDayRange = FloatRange.Zero;

        public SimpleCurve severityCurve;

        public float severityFactor = 1f;

        public float severityOffset = 0f;

        public HediffDef hediff;

        public int updateInterval = 250;

        public float mechanitorFactor = 1f;

        public float minAge = 0f;

    }
}
