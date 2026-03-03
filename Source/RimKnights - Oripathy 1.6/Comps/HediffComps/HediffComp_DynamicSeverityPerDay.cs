using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace RimKnights.Oripathy
{
    public class HediffComp_DynamicSeverityPerDay : HediffComp_SeverityModifierBase
    {
        private HediffCompProperties_DynamicSeverityPerDay Props => (HediffCompProperties_DynamicSeverityPerDay)this.props;

        public override float SeverityChangePerDay()
        {
            if (Pawn.ageTracker.AgeBiologicalYearsFloat < this.Props.minAge)
            {
                return 0f;
            }

            float num = CalculateSeverityPerDay();
            if (OripathyMod.settings.debugMode) Log.Message("DebugSeverityGainPerDay".Translate(Pawn.LabelShort.Named("pawn"), parent.def.label.Named("hediff"), num));
            
            HediffStage curStage = this.parent.CurStage;

            num *= ((curStage != null) ? curStage.severityGainFactor : 1f);

            return num;
        }

        private float CalculateSeverityPerDay()
        {
            List<AffectorHediff> affectorHediffs = Props.AffectorHediffs;

            float totalChange = 0f;
            if(affectorHediffs.NullOrEmpty()) { return totalChange;}

            //Only looks for the first affector in a priority list manner
            for(int i = 0; i < affectorHediffs.Count; i++)
            {
                AffectorHediff affector = affectorHediffs[i];
                Hediff hediff = Pawn.health.hediffSet.GetFirstHediffOfDef(affector.hediff);
                if (hediff != null)
                {
                    float change;
                    if(affector.curve != null)
                    {
                        change = affector.curve.Evaluate(hediff.Severity);
                    }
                    else
                    {
                        change = affector.severityFactor * hediff.Severity + affector.severityOffset;
                    }

                    if (affector.severityScalingStat != null)
                    {
                        change *= (affector.inverseStatScaling ? Mathf.Max(1f - Pawn.GetStatValue(affector.severityScalingStat, true, -1), 0f) : Pawn.GetStatValue(affector.severityScalingStat, true, -1));
                    }
                    totalChange += change;
                    if (!Props.cumulative) return totalChange;
                }

            }
            return totalChange;
        }
    }
}
