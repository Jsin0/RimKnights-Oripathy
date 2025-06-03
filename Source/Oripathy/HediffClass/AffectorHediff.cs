using RimWorld;
using Verse;

namespace RimKnights
{
    public class AffectorHediff
    {
        public HediffDef hediff;

        public SimpleCurve curve;

        public float severityFactor = 1f;

        public float severityOffset = 0f;

        public StatDef severityScalingStat;

        public bool inverseStatScaling = false;

        public FloatRange severityPerDayRange = FloatRange.Zero;
    }
}
