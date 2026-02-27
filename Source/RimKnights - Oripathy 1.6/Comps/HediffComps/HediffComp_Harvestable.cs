using RimWorld;
using Verse;

namespace RimKnights.Oripathy
{
    public class HediffComp_Harvestable : HediffComp
    {
        private int cooldownTicksLeft = 0;
        private bool ready = true;
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
                else if (parent.Severity < Props.minimumSeverity)
                {
                    return $"Hediff severity still below minimum ({Props.minimumSeverity}).";
                }
                else
                {
                    return "Ready to be harvested.";
                }
            }

        }
        private int CooldownTicks
        {
            get
            {
                return Props.cooldownHours * 2500;
            }
        }
        public bool Harvestable
        {
            get
            {
                return Props.resource != null && ready && parent.Severity >= Props.minimumSeverity && parent.Visible;
            }
        }
        public override void CompPostTickInterval(ref float severityAdjustment, int delta)
        {
            base.CompPostTickInterval(ref severityAdjustment, delta);
            if (!ready)
            {
                if ((cooldownTicksLeft -= delta) <= 0)
                {
                    ready = true;
                }
            }
        }
        public void ResetCooldown()
        {
            if(Props.cooldownHours > 0)
            {
                ready = false;
                cooldownTicksLeft = CooldownTicks;
            }

        }
        public override void CompExposeData()
        {
            base.CompExposeData();
            Scribe_Values.Look<bool>(ref this.ready, "ready", true);
            Scribe_Values.Look<int>(ref this.cooldownTicksLeft, "cooldownTicksLeft", CooldownTicks);

        }

    }
}
