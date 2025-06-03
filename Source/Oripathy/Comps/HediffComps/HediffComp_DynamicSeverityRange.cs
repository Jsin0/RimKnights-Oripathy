using RimWorld;
using System;
using System.Text;
using UnityEngine;
using Verse;

namespace RimKnights
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

        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);
            if (base.Pawn.IsHashIntervalTick(this.Props.updateInterval))
            {
                //Log.Message("adjusting min/max severity");
                CalculateLimits();
                AdjustSeverity();
            }

        }

        private void AdjustSeverity()
        {
            //Log.Message("Adjusting Severity");
            float severity = this.parent.Severity;
            float adjustment;
            if (severity < minSeverity)
            {
                adjustment = (minSeverity - severity) / 60000f * this.Props.updateInterval;
                //Log.Message($"severity {severity} {adjustment}");
                //Log.Message(adjustment);
                this.parent.Severity += adjustment;
                //Log.Message("new severity: " + this.parent.Severity);
            }
            else if (severity > maxSeverity)
            {
                adjustment = (severity - maxSeverity) / 60000f * this.Props.updateInterval;
                //Log.Message($"severity {severity} {adjustment}");
                this.parent.Severity -= adjustment;
            }

        }

        public void CalculateLimits()
        {
            float num;

            if (CalculateSeverityCap(Props.minAffector, out num))
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

            if (maxSeverity < minSeverity) { maxSeverity = minSeverity; }

            //Log.Message($"New Limits: {minSeverity} - {maxSeverity}");
        }

        private bool CalculateSeverityCap(AffectorHediff affector, out float cap)
        {
            if (affector?.hediff != null)
            {
                Hediff hediff = this.Pawn.health.hediffSet.GetFirstHediffOfDef(affector.hediff);
                float severity = hediff?.Severity ?? 0f;

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
        public float minSeverity;

        public float maxSeverity;
    }
}
