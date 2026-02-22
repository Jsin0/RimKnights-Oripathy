using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace RimKnights.Oripathy
{
    public class HediffComp_DynamicSeverityPerDay : HediffComp_SeverityModifierBase
    {
        private HediffCompProperties_DynamicSeverityPerDay Props
        {
            get
            {
                return (HediffCompProperties_DynamicSeverityPerDay)this.props;
            }
        }

        public override float SeverityChangePerDay()
        {
            if (Pawn.ageTracker.AgeBiologicalYearsFloat < this.Props.minAge)
            {
                return 0f;
            }

            float num = CalculateSeverityPerDay();
            if (OripathyMod.settings.debugMode) Log.Message($"SeverityGainPerDay".Translate(Pawn.Name.ToStringShort, parent.def.label, num));
            
            HediffStage curStage = this.parent.CurStage;

            num *= ((curStage != null) ? curStage.severityGainFactor : 1f);

            return num;
        }

        private float CalculateSeverityPerDay()
        {
            List<AffectorHediff> affectorHediffs = Props.AffectorHediffs;

            if(affectorHediffs.NullOrEmpty()) { return 0f;}

            for(int i = 0; i < affectorHediffs.Count; i++)
            {
                Hediff hediff = Pawn.health.hediffSet.GetFirstHediffOfDef(affectorHediffs[i]?.hediff);
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
                        severity *= (affector.inverseStatScaling ? Mathf.Max(1f - Pawn.GetStatValue(affector.severityScalingStat, true, -1), 0f) : Pawn.GetStatValue(affector.severityScalingStat, true, -1));
                    }

                    return severity + affector.severityPerDayRange.RandomInRange;
                }

            }
            return 0f;
        }
    }
}
