using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace RimKnights
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

        public override float SeverityChangePerDay()
        {
            if (base.Pawn.ageTracker.AgeBiologicalYearsFloat < this.Props.minAge)
            {
                return 0f;
            }

            float num = CalculateSeverityPerDay();

            HediffStage curStage = this.parent.CurStage;

            num *= ((curStage != null) ? curStage.severityGainFactor : 1f);

            return num;
        }

        private float CalculateSeverityPerDay()
        {
            List<AffectorHediff> affectorHediffs = Props.AffectorHediffs;

            for(int i = 0; i < affectorHediffs.Count; i++)
            {
                Hediff hediff = this.Pawn.health.hediffSet.GetFirstHediffOfDef(affectorHediffs[i]?.hediff);
                if (hediff != null)
                {
                    AffectorHediff affector = affectorHediffs[i];
                    float severity;
                    if(affector.curve != null)
                    {
                        severity = affector.curve.Evaluate(hediff.Severity);
                    }
                    else
                    {
                        severity = affector.severityFactor * hediff.Severity + affector.severityOffset;
                    }

                    if(affector.severityScalingStat != null)
                    {
                        severity *= (affector.inverseStatScaling ? Mathf.Max(1f - this.Pawn.GetStatValue(affector.severityScalingStat, true, -1), 0f) : this.Pawn.GetStatValue(affector.severityScalingStat, true, -1));
                    }

                    return severity + affector.severityPerDayRange.RandomInRange;
                }

            }
            return 0f;
        }
    }
}
