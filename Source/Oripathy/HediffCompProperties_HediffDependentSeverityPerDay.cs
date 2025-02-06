using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace Oripathy
{
    public class HediffCompProperties_HediffDependentSeverityPerDay : HediffCompProperties
    {
        public HediffCompProperties_HediffDependentSeverityPerDay()
        {
            this.compClass = typeof(HediffComp_HediffDependentSeverityPerDay);
        }

        public float CalculateSeverityPerDay()
        {
            float num = this.severityPerDay + this.severityPerDayRange.RandomInRange;

            if (Rand.Chance(this.reverseSeverityChangeChance))
            {
                num *= -1f;
            }

            return num;
        }

        public float severityPerDay;

        public HediffDef hediff;

        public SimpleCurve hediffSeverityToSeverityGainCurve;

        public float mechanitorFactor = 1f;

        public float reverseSeverityChangeChance;

        public FloatRange severityPerDayRange = FloatRange.Zero;

        public float minAge;

    }
}
