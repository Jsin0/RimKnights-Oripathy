using RimWorld;
using System;
using Verse;

namespace RimKnights.Oripathy
{
    public class HediffComp_Suppressible : HediffComp
    {
        private HediffCompProperties_Suppressible Props => (HediffCompProperties_Suppressible)props;
        
        public override void CompPostPostAdd(DamageInfo? dinfo)
        {
            if(Props.suppressor == null || Props.suppressedHediff == null || Props.unsuppressedHediff == null)
            {
                Log.Error("CompSuppresibleConfigError".Translate());
                Pawn.health.RemoveHediff(parent);
                return;
            }
            DoSuppression();
        }

        public override void CompPostTickInterval(ref float severityAdjustment, int delta)
        {
            base.CompPostTickInterval(ref severityAdjustment, delta);
            if (Pawn.IsHashIntervalTick(this.Props.checkInterval, delta))
            {
                DoSuppression();
            }

        }

        private void DoSuppression()
        {
            if (Props.suppressor == null || Props.suppressedHediff == null || Props.unsuppressedHediff == null)
            {
                return;
            }

            HediffDef hediffDef = ((this.IsSuppressed) ? this.Props.suppressedHediff : this.Props.unsuppressedHediff);

            Hediff hediff = Pawn.health.hediffSet.GetFirstHediffOfDef(hediffDef, false);

            if (hediff != null)
            {
                if (this.parent == hediff) { return; }
                hediff.Severity += this.parent.Severity;
            }
            else
            {
                hediff = Pawn.health.GetOrAddHediff(hediffDef);
                hediff.Severity = this.parent.Severity;
            }
            Pawn.health.RemoveHediff(this.parent);

        }
        private bool IsSuppressed => Pawn.health.hediffSet.GetFirstHediffOfDef(this.Props.suppressor) != null; 
    }
}
