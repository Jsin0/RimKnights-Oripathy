using RimWorld;
using System;
using System.Text;
using UnityEngine;
using Verse;

namespace Originium
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
                CalculateLimits();
                AdjustSeverity();
            }

        }

        private void AdjustSeverity()
        {
            Log.Message("Adjusting Severity");
            float severity = this.parent.Severity;
            float adjustment;
            if (severity < minSeverity)
            {
                adjustment = (minSeverity - severity) / 60000f / this.Props.updateInterval;
                Log.Message($"severity {severity} {adjustment}");
                this.parent.Severity += adjustment;
            }
            else if (severity > maxSeverity)
            {
                adjustment = (severity - maxSeverity) / 60000f / this.Props.updateInterval;
                Log.Message($"severity {severity} {adjustment}");
                this.parent.Severity -= adjustment;
            }

        }

        public void CalculateLimits()
        {
            Hediff hediff = this.Pawn.health.hediffSet.GetFirstHediffOfDef(this.Props.hediff);
            float severity = ((hediff != null) ? hediff.Severity : 0f);

            float num;

            if ((num = this.Props.CalculateMinSeverity(severity)) != -88)
            {
                minSeverity = num;
            }
            else
            {
                //Log.Message("no min factor supplied");
                minSeverity = this.parent.def.minSeverity;
            }

            if ((num = this.Props.CalculateMaxSeverity(severity)) != -88)
            {
                maxSeverity = num;
            }
            else
            {
                //Log.Message("no max factor supplied");
                maxSeverity = this.parent.def.maxSeverity;
            }


            if (maxSeverity < minSeverity) { maxSeverity = minSeverity; }
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
