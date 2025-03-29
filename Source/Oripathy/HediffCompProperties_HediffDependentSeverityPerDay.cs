using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace Originium
{
    public class HediffCompProperties_HediffDependentSeverityPerDay : HediffCompProperties
    {
        public HediffCompProperties_HediffDependentSeverityPerDay()
        {
            this.compClass = typeof(HediffComp_HediffDependentSeverityPerDay);
        }

        public float CalculateSeverityPerDay(float affectorSeverity = 0f, bool suppressed = false)
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
             
            if (suppressed)
            {
                sev *= this.suppressionFactor;
            }

            return sev + this.severityPerDayRange.RandomInRange;
        }

        public FloatRange severityPerDayRange = FloatRange.Zero;

        public SimpleCurve severityCurve;

        public float severityFactor = 1f;

        public float severityOffset = 0f;

        public HediffDef hediff;

        public HediffDef suppressorHediff;

        public float suppressionFactor = 0.1f;

        public int updateInterval = 250;

        public float mechanitorFactor = 1f;

        public float minAge = 0f;

    }
}
