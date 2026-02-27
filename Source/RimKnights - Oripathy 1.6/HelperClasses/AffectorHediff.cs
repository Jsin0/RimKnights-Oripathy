using RimWorld;
using Verse;

namespace RimKnights.Oripathy
{
    public class AffectorHediff
    {
        public HediffDef hediff;

        public SimpleCurve curve;

        public float severityFactor = 0f;

        public float severityOffset = 0f;

        public StatDef severityScalingStat;

        public bool inverseStatScaling = false;

        public FloatRange severityPerDayRange = FloatRange.Zero;
    }
}
