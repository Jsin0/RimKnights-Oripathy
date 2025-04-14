using RimWorld;
using System;
using Verse;

namespace Originium
{
    public class HediffCompProperties_DynamicSeverityRange : HediffCompProperties
    {
        public HediffCompProperties_DynamicSeverityRange()
        {
            this.compClass = typeof(HediffComp_DynamicSeverityRange);
        }

        public float CalculateMinSeverity(float affectorSeverity)
        {
            return CalculateSeverity(minSeverityCurve, minSeverityFactor, affectorSeverity, minSeverityOffset);
        }
        public float CalculateMaxSeverity(float affectorSeverity)
        {
            return CalculateSeverity(maxSeverityCurve, maxSeverityFactor, affectorSeverity, maxSeverityOffset);
        }

        private float CalculateSeverity(SimpleCurve curve, float factor, float xValue = 0f, float offset = 0f)
        {
            if (curve == null)
            {
                if (factor == null || factor == 0)
                {
                    return -88;
                }
                else
                {
                    return xValue * factor + offset;
                }
            }
            else
            {
                return curve.Evaluate(xValue);
            }
        }

        public HediffDef hediff;

        public SimpleCurve minSeverityCurve;

        public SimpleCurve maxSeverityCurve;

        public float minSeverityFactor;

        public float minSeverityOffset;

        public float maxSeverityFactor;

        public float maxSeverityOffset;

        public int updateInterval = 250;

        public float adjustment = 0f;
    }
}
