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
            return this.hediffSeverityToSeverityGainCurve.Evaluate(0f);
        }

        public bool showDaysToRecover;

        public bool showHoursToRecover;

        public HediffDef hediff;

        public SimpleCurve hediffSeverityToSeverityGainCurve;

        public float mechanitorFactor = 1f;

        public float minAge = 0f;

    }
}
