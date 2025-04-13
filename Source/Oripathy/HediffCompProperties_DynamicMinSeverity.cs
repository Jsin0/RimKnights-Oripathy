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

        public float CalculateMinSeverity(float affectorSeverity)
        {
            return CalculateSeverity( severityCurveMin, severityFactorMin,affectorSeverity, severityOffsetMin);
        }
        public float CalculateMaxSeverity(float affectorSeverity)
        {
            return CalculateSeverity(severityCurveMax, severityFactorMax, affectorSeverity, severityOffsetMax);
        } 

        private float CalculateSeverity( SimpleCurve curve, float factor ,float xValue = 0f, float offset = 0f)
        {
            if (curve == null)
            {
                if(factor == null)
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

        public SimpleCurve severityCurveMin;

        public SimpleCurve severityCurveMax;

        public float severityFactorMin;

        public float severityOffsetMin;

        public float severityFactorMax;

        public float severityOffsetMax;

        public int updateInterval = 250;

        public float adjustment = 0f;
    }
}
