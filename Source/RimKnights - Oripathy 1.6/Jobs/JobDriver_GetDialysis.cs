using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;


namespace RimKnights.Oripathy
{
    public class JobDriver_GetDialysis : JobDriver
    {
        private const TargetIndex ChargerInd = TargetIndex.A;
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return this.pawn.Reserve(this.job.GetTarget(TargetIndex.A).Thing, this.job, 1, -1, null, errorOnFailed, false);
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            Building machine = null;
            CompDialysisMachine comp = null;
            CompPowerTrader power = null;
            float severityReductionPerTick = 0.20f;
            bool shareReduction =false;

            this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
            this.FailOn(() => power != null && !power.PowerOn);

            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.InteractionCell, false);
            yield return Toils_General.Wait(30f.SecondsToTicks(), TargetIndex.None).WithProgressBarToilDelay(TargetIndex.A);

            Toil dialysis = ToilMaker.MakeToil("Dialysis");
            dialysis.handlingFacing = true;
            dialysis.defaultCompleteMode = ToilCompleteMode.Delay;
            dialysis.defaultDuration = 8 * 2500;
            dialysis.initAction = delegate
            {
                machine = job.GetTarget(TargetIndex.A).Thing as Building;
                comp = machine.GetComp<CompDialysisMachine>();
                power = machine.GetComp<CompPowerTrader>();
                shareReduction = comp.Props.shareSeverityReduction;
                severityReductionPerTick = comp.Props.severityReductionPerHour / Utilities.Constants.TicksPerHour * Utilities.Constants.TicksPerRareTick;
                pawn.Rotation = machine.Rotation;
            };
            dialysis.WithProgressBarToilDelay(TargetIndex.A);
            dialysis.tickAction = delegate{
                if (pawn.IsHashIntervalTick(250))
                {
                    int count = 0;
                    foreach (HediffDef def in comp.Props.hediffs)
                    {
                        Hediff hediff = pawn.health.hediffSet.GetFirstHediffOfDef(def);
                        if (hediff != null && hediff.Severity > 0)
                        {
                            if (!shareReduction)
                            {
                                hediff.Severity = MathF.Max(0f, hediff.Severity - severityReductionPerTick);
                                continue;
                            }
                            count++;
                        }

                    }

                    if(shareReduction && count > 0)
                    {
                        float splitReduction = severityReductionPerTick / count;
                        foreach (HediffDef def in comp.Props.hediffs)
                        {
                            Hediff hediff = pawn.health.hediffSet.GetFirstHediffOfDef(def);
                            if (hediff != null && hediff.Severity > 0)
                            {
                                hediff.Severity = MathF.Max(0f, hediff.Severity - splitReduction);
                            }

                        }
                    }
                }
            };
            yield return dialysis;

            yield return Toils_General.Wait(0.35f.SecondsToTicks(), TargetIndex.None);
            yield break;
        }

    }
}
