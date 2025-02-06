using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace Oripathy
{
    public class HediffComp_HediffDependentSeverityPerDay : HediffComp_SeverityModifierBase
    {
        private HediffCompProperties_HediffDependentSeverityPerDay Props
        {
            get
            {
                return (HediffCompProperties_HediffDependentSeverityPerDay)this.props;
            }
        }

        public override void CompPostPostAdd(DamageInfo? dinfo)
        {
            base.CompPostPostAdd(dinfo);
            this.severityPerDay = this.Props.CalculateSeverityPerDay();
        }
        public override float SeverityChangePerDay()
        {
            if (base.Pawn.ageTracker.AgeBiologicalYearsFloat < this.Props.minAge)
            {
                return 0f;
            }
            float num = this.severityPerDay;

            Hediff affectorHediff = this.Pawn.health.hediffSet.GetFirstHediffOfDef(this.Props.hediff);
            
            if(affectorHediff != null)
            {
                num = this.Props.hediffSeverityToSeverityGainCurve.Evaluate(affectorHediff.Severity);
            }
            else
            {
                num = severityPerDay;
            }
            
            HediffStage curStage = this.parent.CurStage;

            float num2 = num * ((curStage != null) ? curStage.severityGainFactor : 1f);

            return num2;
        }

        public float severityPerDay;

    }


}
