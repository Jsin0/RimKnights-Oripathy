using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using Verse.Sound;

namespace RimKnights.Oripathy
{
    public class JobDriver_Infect : JobDriver
    {
        protected Pawn Target
        {
            get
            {
                return (Pawn)this.job.GetTarget(TargetIndex.A).Thing;
            }
        }
        public static bool AvailableOnNow(Pawn pawn, BodyPartRecord part = null)
        {
            return pawn.RaceProps.Humanlike && (Faction.OfPlayerSilentFail == null) && (part == null) && !pawn.health.hediffSet.HasHediff(HediffDefOf.RK_Oripathy);
        }

        // Token: 0x060042D7 RID: 17111 RVA: 0x0018B562 File Offset: 0x00189762
        public static IEnumerable<BodyPartRecord> GetPartsToApplyOn(Pawn pawn)
        {
            foreach (BodyPartRecord bodyPartRecord in pawn.health.hediffSet.GetNotMissingParts(BodyPartHeight.Undefined, BodyPartDepth.Undefined, null, null))
            {
                if (bodyPartRecord.def.canScarify)
                {
                    yield return bodyPartRecord;
                }
            }
            IEnumerator<BodyPartRecord> enumerator = null;
            yield break;
            yield break;
        }
        public static void Infect(Pawn pawn, BodyPartRecord part)
        {
            if (!ModLister.CheckIdeology("Oripathy Infection"))
            {
                return;
            }
            Lord lord = pawn.GetLord();
            LordJob_Ritual_Mutilation lordJob_Ritual_Mutilation;
            if (lord != null && (lordJob_Ritual_Mutilation = lord.LordJob as LordJob_Ritual_Mutilation) != null)
            {
                lordJob_Ritual_Mutilation.mutilatedPawns.Add(pawn);
            }
            pawn.health.AddHediff(HediffDefOf.RK_Oripathy, null, null, null);
            pawn.health.AddHediff(HediffDefOf.RK_OripathyLesion, part, null, null);
        }
        public static void CreateHistoryEventDef(Pawn pawn)
        {
            Find.HistoryEventsManager.RecordEvent(new HistoryEvent(HistoryEventDefOf.RK_BecameOripathic_Ritual, pawn.Named(HistoryEventArgsNames.Doer)), true);
        }
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return this.pawn.Reserve(this.Target, this.job, 1, -1, null, errorOnFailed, false);
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            if (!ModLister.CheckIdeology("Make Infected"))
            {
                yield break;
            }
            this.FailOnDespawnedOrNull(TargetIndex.A);
            Pawn target = this.Target;
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.ClosestTouch, false);
            yield return Toils_General.Wait(35, TargetIndex.None);
            Toil toil = ToilMaker.MakeToil("MakeNewToils");
            toil.initAction = delegate
            {
                IEnumerable<BodyPartRecord> partsToApplyOn = JobDriver_Infect.GetPartsToApplyOn(target);
                Func<BodyPartRecord, bool> availablePartCheck = part => JobDriver_Infect.AvailableOnNow(target, part);
                BodyPartRecord bodyPartRecord;
                if (!partsToApplyOn.Where(availablePartCheck).TryRandomElement(out bodyPartRecord) && !JobDriver_Infect.GetPartsToApplyOn(target).TryRandomElement(out bodyPartRecord))
                {
                    this.pawn.jobs.EndCurrentJob(JobCondition.Errored, true, true);
                    Log.Error("Failed to find body part to infect");
                }
                JobDriver_Infect.Infect(target, bodyPartRecord);
                JobDriver_Infect.CreateHistoryEventDef(target);
                SoundDefOf.Execute_Cut.PlayOneShot(target);
                if (target.RaceProps.BloodDef != null)
                {
                    CellRect cellRect = new CellRect(target.PositionHeld.x - 1, target.PositionHeld.z - 1, 3, 3);
                    for (int i = 0; i < 3; i++)
                    {
                        IntVec3 randomCell = cellRect.RandomCell;
                        if (randomCell.InBounds(this.Map) && GenSight.LineOfSight(randomCell, target.PositionHeld, this.Map))
                        {
                            FilthMaker.TryMakeFilth(randomCell, target.MapHeld, target.RaceProps.BloodDef, target.LabelIndefinite(), 1, FilthSourceFlags.None);
                        }
                    }
                }
            };
            toil.defaultCompleteMode = ToilCompleteMode.Instant;
            yield return toil;
            yield break;
        }
        private const TargetIndex TargetPawnIndex = TargetIndex.A;
    }
}
