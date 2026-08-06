using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse.AI;
using Verse;

namespace RimKnights.Oripathy
{
    public class JobDriver_TakeToDialysisMachine : JobDriver
    {
        protected Pawn Takee
        {
            get
            {
                return (Pawn)this.job.GetTarget(TargetIndex.B).Thing;
            }
        }

        protected ThingWithComps Dialyzer
        {
            get
            {
                return (ThingWithComps)this.job.GetTarget(TargetIndex.A).Thing;
            }
        }

        private bool TakeeRescued
        {
            get
            {
                return this.Takee.RaceProps.Humanlike && this.Takee.ageTracker.CurLifeStage.alwaysDowned;
            }
        }

        public override string GetReport()
        {
            if (!this.TakeeRescued)
            {
                return "TakingToDialyzer".Translate(this.Takee);
            }
            return base.GetReport();
        }

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            this.Takee.ClearAllReservations(true);
            return this.pawn.Reserve(this.Takee, this.job, 1, -1, null, errorOnFailed, false) && this.pawn.Reserve(this.Dialyzer, this.job, 1, 0, null, errorOnFailed, false);
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDestroyedOrNull(TargetIndex.A);
            this.FailOnDestroyedOrNull(TargetIndex.B);
            this.FailOnAggroMentalStateAndHostile(TargetIndex.B);
        }
    }
}
