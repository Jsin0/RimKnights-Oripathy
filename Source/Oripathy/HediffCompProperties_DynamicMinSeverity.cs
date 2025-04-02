using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;

namespace Originium
{
    public class HediffCompProperties_DynamicSeverityRange : HediffCompProperties
    {
        public HediffCompProperties_DynamicSeverityRange()
        {
            this.compClass = typeof(HediffComp_DynamicSeverityRange);
        }

        public float CalculateMinSeverity(float affectorSeverity = 0f)
        {
            if(severityCurveMin == null)
            {
                if (severityFactorMin == null)
                {
                    return -8;
                }
                else
                {
                    return affectorSeverity * severityFactorMin + severityOffsetMin;
                }
            }
            else
            {
                return severityCurveMin.Evaluate(affectorSeverity);
            }
        }
        public float CalculateMaxSeverity(float affectorSeverity = 0f)
        {
            if (severityCurveMax == null)
            {
                if(severityFactorMax == null)
                {
                    return -1;
                }
                else
                {
                    return affectorSeverity * severityFactorMax + severityOffsetMax;
                }
            }
            else
            {
                return severityCurveMax.Evaluate(affectorSeverity);
            }
        }

        public HediffDef hediff;

        public float minSeverity = 0f;

        public float maxSeverity = 1f;

        public SimpleCurve severityCurveMin;

        public SimpleCurve severityCurveMax;

        public float severityFactorMin;

        public float severityOffsetMin = 0f;

        public float severityFactorMax;

        public float severityOffsetMax = 0f;

        public int updateInterval = 250;
    }
}
