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
        public override bool Visible
        {
            get
            {
                return base.Visible && (!CoreMod.settings.infectionMonitor && (this.CurStage == null || this.CurStage.becomeVisible));
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
