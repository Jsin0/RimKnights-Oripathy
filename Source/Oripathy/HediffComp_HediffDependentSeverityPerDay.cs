using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using UnityEngine;
using Verse;

namespace Originium
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

        public override void CompExposeData()
        {
            base.CompExposeData();
            Scribe_Values.Look<float>(ref this.severityPerDay, "severityPerDay", 0f, false);
            if (Scribe.mode == LoadSaveMode.PostLoadInit && this.severityPerDay == 0f)
            {
                this.severityPerDay = this.RecalculateChangePerDay();
                Log.Warning("Hediff " + this.parent.Label + " had severityPerDay not matching parent.");
            }
        }
        public override float SeverityChangePerDay()
        {
            if (base.Pawn.ageTracker.AgeBiologicalYearsFloat < this.Props.minAge)
            {
                return 0f;
            }

            float num = this.severityPerDay;

            if (base.Pawn.IsHashIntervalTick(this.Props.updateInterval))
            {
                this.severityPerDay = RecalculateChangePerDay();
            }

            HediffStage curStage = this.parent.CurStage;

            float num2 = num * ((curStage != null) ? curStage.severityGainFactor : 1f);

            //Log.Message("Severity per day: " + num);
            return num2;
        }


        public float RecalculateChangePerDay()
        {
            Hediff affectorHediff = this.Pawn.health.hediffSet.GetFirstHediffOfDef(this.Props.hediff);
            bool isSuppressed = this.Pawn.health.hediffSet.HasHediff(this.Props.suppressorHediff);

            float severity = ((affectorHediff != null) ? affectorHediff.Severity : 0f);

            return this.Props.CalculateSeverityPerDay(severity, isSuppressed);
        }

        public float severityPerDay;

    }


}
