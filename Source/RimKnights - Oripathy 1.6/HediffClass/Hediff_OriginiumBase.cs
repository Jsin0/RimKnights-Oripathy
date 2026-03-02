using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using UnityEngine;
using Verse;

namespace RimKnights.Oripathy
{
    public abstract class Hediff_OriginiumBase : HediffWithComps
    {
        private bool shouldUpdate;
        private bool revealed;
        public override string SeverityLabel
        {
            get
            {
                if (OripathyMod.settings.infectionMonitor)
                {
                    if (this.displayedSeverity <= 0f)
                    {
                        return null;
                    }
                    else
                    {
                        return this.displayedSeverity.ToStringPercent("F0") + (shouldUpdate ? null : "*");
                    }
                }
                else
                {
                    return base.SeverityLabel;
                }
            }
        }
        public override string LabelInBrackets
        {
            get
            {
                if(!OripathyMod.settings.infectionMonitor || shouldUpdate)
                {
                    return base.LabelInBrackets;
                }
                return null;
            }
        }
        public float displayedSeverity 
        {
            get;
            private set;
        }
        public override void PostAdd(DamageInfo? dinfo)
        {
            if (!this.pawn.RaceProps.IsFlesh)
            {
                this.pawn.health.RemoveHediff(this);
                return;
            }
            else if (OripathyMod.settings.baselinersImmune && GeneUtility.IsBaseliner(this.pawn))
            {
                this.pawn.health.RemoveHediff(this);
                return;
            }
            base.PostAdd(dinfo);
            displayedSeverity = this.Severity;
        }
        public override bool Visible
        {
            get
            {
                if (!OripathyMod.settings.infectionMonitor)
                {
                    return base.Visible;
                }

                if (revealed) return true;

                if (base.Visible || shouldUpdate)
                {
                    revealed = true;
                    return true;
                }

                return false;
            }
        }
        public override void Tick()
        {
            base.Tick();
            if (OripathyMod.settings.infectionMonitor)
            {
                if(pawn.IsHashIntervalTick(60))
                {
                    shouldUpdate = pawnIsWearingMonitor;
                    if (shouldUpdate) RefreshDisplayedSeverity();
                }
            }
        }
        private bool pawnIsWearingMonitor => pawn.health.hediffSet.HasHediff(HediffDefOf.RK_InfectionMonitorImplant) || pawn.health.hediffSet.HasHediff(HediffDefOf.RK_InfectionMonitorWorn);
        public void RefreshDisplayedSeverity()
        {
            this.displayedSeverity = this.Severity;
        }
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref revealed, "revealed", false);
            if(Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                displayedSeverity = this.Severity;
                shouldUpdate = pawnIsWearingMonitor;
            }
        }
    }
}
