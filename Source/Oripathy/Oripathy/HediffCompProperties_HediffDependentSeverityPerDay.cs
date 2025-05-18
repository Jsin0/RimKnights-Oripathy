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

        public float CalculateSeverityPerDay(float affectorSeverity = 0f, bool usePrimary = true)
        {
            SimpleCurve curve = severityCurve;
            float factor = severityFactor;
            float offset = severityOffset;

            if (!usePrimary)
            {
                curve = severityCurve2;
                factor = severityFactor2;
                offset = severityOffset2;
            }

            float sev;
            if (curve != null)
            {
                sev = curve.Evaluate(affectorSeverity);
            }
            else
            {
                sev = (factor * affectorSeverity) + offset;
            }

            return sev + this.severityPerDayRange.RandomInRange;
        }

        public FloatRange severityPerDayRange = FloatRange.Zero;

        public HediffDef primaryHediff;

        public SimpleCurve severityCurve;

        public float severityFactor = 1f;

        public float severityOffset = 0f;

        public HediffDef secondHediff;

        public SimpleCurve severityCurve2;

        public float severityFactor2 = 1f;

        public float severityOffset2 = 0f;

        public int updateInterval = 250;

        public float mechanitorFactor = 1f;

        public float minAge = 0f;

    }
}
