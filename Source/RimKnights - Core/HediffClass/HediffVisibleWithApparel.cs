using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using UnityEngine;
using Verse;

namespace RimKnights
{
    public class HediffVisibleWithApparel : HediffWithComps
    {
        public virtual string SeverityLabel
        {
            get
            {
                if ((!this.IsLethal && !this.def.alwaysShowSeverity) || this.cachedSeverity <= 0f)
                {
                    return null;
                }

                if (this.shouldUpdate)
                {
                    cachedSeverity = this.Severity;
                }
                return (this.cachedSeverity / Mathf.Abs(this.def.lethalSeverity)).ToStringPercent("F0");
            }
        }
        public override void PostAdd(DamageInfo? dinfo)
        {
            if (!this.pawn.RaceProps.IsFlesh)
            {
                this.pawn.health.RemoveHediff(this);
                return;
            }
            else if (GeneUtility.IsBaseliner(this.pawn) && CoreMod.baselinersImmune)
            {
                this.pawn.health.RemoveHediff(this);
                return;
            }

            base.PostAdd(dinfo);
        }
        public override bool Visible
        {
            get
            {
                if (CoreMod.infectionMonitor)
                {
                    return shouldUpdate;
                }
                else
                {
                    return base.Visible;
                }
            }
        }
        public override void Tick()
        {
            base.Tick();
            if (pawn.IsHashIntervalTick(200))
            {
                shouldUpdate = pawnIsWearingMonitor;
            }
        }
        private bool pawnIsWearingMonitor
        {
            get { return pawn.apparel.WornApparel.Any(app => app.HasComp<CompInfectionMonitor>()); }
        }

        private bool shouldUpdate;

        private float cachedSeverity;
    }
}
