using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace RimKnights
{
    public class HediffComp_Harvestable : HediffComp
    {
        public HediffCompProperties_Harvestable Props
        {
            get
            {
                return (HediffCompProperties_Harvestable)this.props;
            }
        }
        public override string CompTipStringExtra
        {
            get
            {
                if (!ready)
                {
                    return $"recovering: {GenDate.ToStringTicksToPeriod(cooldownTicksLeft)} left.";
                }
                else
                {
                    return null;
                }
            }

        }
        private int cooldownTicks
        {
            get
            {
                return Props.cooldownHours * 2500;
            }
        }
        public bool harvestable
        {
            get
            {
                return Props.resource != null && ready && parent.Severity >= Props.miniumSeverity;
            }
        }
        public override void CompPostTick(ref float severityAdjustment)
        {
            if (!ready)
            {
                if (parent.pawn.IsHashIntervalTick(250))
                {
                    if ((cooldownTicksLeft -= 250) <= 0)
                    {
                        ready = true;
                    }
                }
            }
            base.CompPostTick(ref severityAdjustment);
        }
        public void ResetCooldown()
        {
            if(Props.cooldownHours > 0)
            {
                ready = false;
                cooldownTicksLeft = cooldownTicks;
            }

        }
        public override void CompExposeData()
        {
            base.CompExposeData();
            Scribe_Values.Look<bool>(ref this.ready, "ready", true);
            Scribe_Values.Look<int>(ref this.cooldownTicksLeft, "cooldownTicksLeft", cooldownTicks);

        }

        private int cooldownTicksLeft = 0;

        public bool ready = true;
    }
}
