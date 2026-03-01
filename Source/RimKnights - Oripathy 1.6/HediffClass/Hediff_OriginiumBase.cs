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
        public override string SeverityLabel
        {
            get
            {
                if (OripathyMod.infectionMonitor)
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
                if(!OripathyMod.infectionMonitor || shouldUpdate)
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
            else if (OripathyMod.baselinersImmune && GeneUtility.IsBaseliner(this.pawn))
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
                if (!OripathyMod.infectionMonitor)
                {
                    return base.Visible;
                }
                return base.Visible || shouldUpdate;
            }
        }
        public override void Tick()
        {
            base.Tick();
            if (OripathyMod.infectionMonitor)
            {
                if(pawn.IsHashIntervalTick(60))
                {
                    shouldUpdate = pawnIsWearingMonitor;
                    if (shouldUpdate) this.displayedSeverity = this.Severity;
                }
            }
        }
        private bool pawnIsWearingMonitor
        {
            get 
            {
                return pawn.health.hediffSet.HasHediff(HediffDefOf.RK_InfectionMonitorImplant) || pawn.health.hediffSet.HasHediff(HediffDefOf.RK_InfectionMonitorWorn);
            }
        }

        private bool shouldUpdate;

    }
}
