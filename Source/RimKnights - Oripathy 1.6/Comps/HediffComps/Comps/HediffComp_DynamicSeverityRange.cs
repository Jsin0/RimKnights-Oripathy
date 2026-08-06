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
            if (Pawn.IsHashIntervalTick(Props.updateInterval, delta))
            {
                CalculateLimits();
                AdjustSeverity();
            }
            if (OripathyMod.settings.debugMode && OripathyMod.settings.verboseLogging && Pawn.Spawned && Pawn.IsHashIntervalTick(60)) Log.Message("DynamicSeverityRangeLimits".Translate(Pawn.Named("PAWN"), parent.LabelCap.Named("HEDIFF"), minSeverity.Named("MIN"), maxSeverity.Named("MAX")));

        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);
            if (Pawn.IsHashIntervalTick(Props.updateInterval))
            {
                CalculateLimits();
                AdjustSeverity();
            }
            if (OripathyMod.settings.debugMode && OripathyMod.settings.verboseLogging && Pawn.Spawned && Pawn.IsHashIntervalTick(60)) Log.Message("DynamicSeverityRangeLimits".Translate(Pawn.Named("PAWN"), parent.LabelCap.Named("HEDIFF"), minSeverity.Named("MIN"), maxSeverity.Named("MAX")));
        }

        private void AdjustSeverity()
        {
            float severity = parent.Severity;
            if (severity >= minSeverity && severity <= maxSeverity) return;

            float target;
            AffectorHediff affectorHediff;
            if (severity < minSeverity)
            {
                target = minSeverity;
                affectorHediff = Props.minAffector;
            }
            else
            {
                target = maxSeverity;
                affectorHediff = Props.maxAffector;
            }

            float maxDelta = 0.1f * Props.updateInterval / Utilities.Constants.TicksPerDay;
            if (affectorHediff.severityScalingStat != null)
            {
                maxDelta *= (affectorHediff.inverseStatScaling ? Mathf.Max(1f - Pawn.GetStatValue(affectorHediff.severityScalingStat, true, -1), 0f) : Pawn.GetStatValue(affectorHediff.severityScalingStat, true, -1));
            }
            
            //Slow convergence so it takes about a day to reach the target
            parent.Severity = Mathf.MoveTowards(severity, target, maxDelta);

        }

        public void CalculateLimits()
        {

            if (CalculateSeverityCap(Props.minAffector, out float minCap))
            {
                minSeverity = minCap;
            }
            else
            {
                minSeverity = parent.def.minSeverity;
            }

            if (CalculateSeverityCap(Props.maxAffector, out float maxCap))
            {
                maxSeverity = maxCap;
            }
            else
            {
                maxSeverity = parent.def.maxSeverity;
            }

            if (maxSeverity < minSeverity) 
            {
                //if (OripathyMod.settings.debugMode && Pawn.Spawned && Pawn.IsHashIntervalTick(60)) Log.Warning("MaxSeverityLessThanMin".Translate());
                maxSeverity = minSeverity; 
            }

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
