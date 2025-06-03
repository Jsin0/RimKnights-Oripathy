using RimWorld;
using System;
using Verse;

namespace RimKnights
{
    public class HediffComp_Suppressible : HediffComp
    {
        private HediffCompProperties_Suppressible Props
        {
            get
            {
                return (HediffCompProperties_Suppressible)this.props;
            }
        }
        public override void CompPostPostAdd(DamageInfo? dinfo)
        {
            doSuppression();
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);
            if (base.Pawn.IsHashIntervalTick(this.Props.checkInterval))
            {
                doSuppression();
            }

        }

        private void doSuppression()
        {
            HediffDef hediffDef = ((this.isSuppressed()) ? this.Props.suppressedHediff : this.Props.unsuppressedHediff);

            if (hediffDef == null)
            {
                Log.Message("HediffComp_Suppressible Error: this comp is suppressed or unsupressed without a replacement hediff");
                return;
            }

            Hediff hediff = base.Pawn.health.hediffSet.GetFirstHediffOfDef(hediffDef, false);

            if (hediff != null)
            {
                if (this.parent == hediff) { return; }
                hediff.Severity += this.parent.Severity;
            }
            else
            {
                hediff = base.Pawn.health.GetOrAddHediff(hediffDef);
                hediff.Severity = this.parent.Severity;
            }
            base.Pawn.health.RemoveHediff(this.parent);

        }

        private bool isSuppressed()
        {
            Hediff suppressor = base.Pawn.health.hediffSet.GetFirstHediffOfDef(this.Props.suppressor);
            if (suppressor != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
