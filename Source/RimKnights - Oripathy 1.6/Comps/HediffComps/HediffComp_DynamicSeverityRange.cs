using RimWorld;
using System;
using System.Text;
using UnityEngine;
using Verse;

namespace RimKnights.Oripathy
{
    public class HediffComp_DynamicSeverityRange : HediffComp
    {
        private HediffCompProperties_DynamicSeverityRange Props
        {
            get
            {
                return (HediffCompProperties_DynamicSeverityRange)this.props;
            }
        }

        public override void CompPostPostAdd(DamageInfo? dinfo)
        {
            base.CompPostPostAdd(dinfo);
            CalculateLimits();
            AdjustSeverity();
        }

        public override void CompPostTickInterval(ref float severityAdjustment, int delta)
        {
            base.CompPostTickInterval(ref severityAdjustment, delta);
            if (base.Pawn.IsHashIntervalTick(this.Props.updateInterval, delta))
            {
                CalculateLimits();
                AdjustSeverity();
            }

        }

        private void AdjustSeverity()
        {
            float severity = this.parent.Severity;
            float target;
            if (severity < minSeverity)
            {
                target = minSeverity;
            }
            else if (severity > maxSeverity)
            {
                target = maxSeverity;
            }
            else { return; }

            //Slow convergence so it takes about a day to reach the target
            parent.Severity = Mathf.MoveTowards(severity, target, 0.02f);

        }

        public void CalculateLimits()
        {

            if (CalculateSeverityCap(Props.minAffector, out float num))
            {
                minSeverity = num;
            }
            else
            {
                minSeverity = this.parent.def.minSeverity;
            }

            if (CalculateSeverityCap(Props.maxAffector, out num))
            {
                maxSeverity = num;
            }
            else
            {
                maxSeverity = this.parent.def.maxSeverity;
            }

            if (maxSeverity < minSeverity) 
            {
                if (OripathyMod.settings.debugMode) Log.Warning("MaxSeverityLessThanMin".Translate());
                maxSeverity = minSeverity; 
            }

            if(OripathyMod.settings.debugMode) Log.Message("DynamicSeverityRangeLimits".Translate(Pawn.Named("PAWN"),parent.LabelCap.Named("HEDIFF"),minSeverity.Named("MIN"), maxSeverity.Named("MAX")));
        }

        private bool CalculateSeverityCap(AffectorHediff affector, out float cap)
        {
            if (affector?.hediff != null)
            {
                float severity;
                Hediff hediff = this.Pawn.health.hediffSet.GetFirstHediffOfDef(affector.hediff);

                if(hediff == null)
                {
                    severity = 0;
                }
                else
                {
                    severity = hediff.Severity;
                }

                if (affector.curve != null)
                {
                    cap = affector.curve.Evaluate(severity);
                }
                else
                {
                    cap = severity * affector.severityFactor + affector.severityOffset;
                }
                return true;
            }
            else
            {
                cap = 0f;
                return false;
            }
        }

        public override void CompExposeData()
        {
            base.CompExposeData();
            Scribe_Values.Look<float>(ref this.minSeverity, "minSeverity", 0f, false);
            Scribe_Values.Look<float>(ref this.maxSeverity, "maxSeverity", 0f, false);
        }

        public override string CompDebugString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(base.CompDebugString());

            stringBuilder.AppendLine("Minimum severity: " + this.minSeverity.ToString("0.##"));
            stringBuilder.AppendLine("Maximum severity: " + this.maxSeverity.ToString("0.##"));

            return stringBuilder.ToString();

        }

        private float minSeverity;

        private float maxSeverity;
    }
}
