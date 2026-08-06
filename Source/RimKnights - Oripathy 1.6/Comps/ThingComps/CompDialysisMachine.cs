using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace RimKnights.Oripathy
{
    [StaticConstructorOnStartup]
    public class CompDialysisMachine : ThingComp
    {
        private static Texture2D colonistOnlyCommandTex;
        public bool allowGuests;
        public CompDialysisMachine.AutoUseMode autoUseMode = CompDialysisMachine.AutoUseMode.AutoUseForEveryone;

        public enum AutoUseMode
        {
            NoAutoUse,
            AutoUseForEveryone
        }
        public CompProperties_DialysisMachine Props => (CompProperties_DialysisMachine)this.props;
        private bool PowerOn => this.parent.TryGetComp<CompPowerTrader>().PowerOn;
        private static Texture2D ColonistOnlyCommandTex
        {
            get
            {
                if(CompDialysisMachine.colonistOnlyCommandTex == null)
                {
                    CompDialysisMachine.colonistOnlyCommandTex = ContentFinder<Texture2D>.Get("UI/Gizmos/NeuralSupercharger_AllowGuests", true);
                }
                return CompDialysisMachine.colonistOnlyCommandTex;
            }
        }

        public bool CanAutoUse(Pawn pawn)
        {
            if (!allowGuests && pawn.IsQuestLodger())
            {
                return false;
            }
            switch (autoUseMode)
            {
                case CompDialysisMachine.AutoUseMode.NoAutoUse:
                    return false;
                case CompDialysisMachine.AutoUseMode.AutoUseForEveryone:
                    return true;
                default:
                    Log.Error(string.Format("Unknown auto use mode: {0}", autoUseMode));
                    return false;
            }
        }
        public override void Initialize(CompProperties props)
        {
            base.Initialize(props);
            autoUseMode = CompDialysisMachine.AutoUseMode.AutoUseForEveryone;
        }
        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look<CompDialysisMachine.AutoUseMode>(ref autoUseMode, "autoUseMode", CompDialysisMachine.AutoUseMode.AutoUseForEveryone, false);
            Scribe_Values.Look<bool>(ref allowGuests, "allowGuests", false, false);
        }
        public override IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn selPawn)
        {
            if(selPawn.CurJob != null && selPawn.CurJob.def == JobDefOf.RK_GetDialysis && selPawn.CurJob.targetA.Thing == parent)
            {
                yield return new FloatMenuOption(Props.jobString + " (" + "RK_GetDialysisAlreadyGetting".Translate() + ")", null, MenuOptionPriority.Default, null, null, 0f, null, null, true, 0);
                yield break;
            }

            bool hasAnyHediff = false;
            foreach(HediffDef def in Props.hediffs)
            {
                Hediff hediff = selPawn.health.hediffSet.GetFirstHediffOfDef(def);
                if (hediff != null && hediff.Severity > 0)
                {
                    hasAnyHediff = true;
                    break;
                }
            }

            if (!hasAnyHediff)
            {
                yield return new FloatMenuOption(Props.jobString + "( " + "RK_GetDialysisNoHediffsToTreat".Translate() + ")", null);
                yield break;
            }

            yield return FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption(Props.jobString, delegate
            {
                Job job = JobMaker.MakeJob(JobDefOf.RK_GetDialysis, parent);
                selPawn.jobs.TryTakeOrderedJob(job, new JobTag?(JobTag.Misc), false);
            }, MenuOptionPriority.Default, null, null, 0f, null, null, true, 0), selPawn, parent, "ReservedBy", null);
            yield break;

        }
        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            foreach (Gizmo gizmo in base.CompGetGizmosExtra()) { yield return gizmo; }

            IEnumerable<Gizmo> enumerator = null;
            yield return new Command_SetDialysisMachineAutoUse(this);
            yield return new Command_Toggle
            {
                defaultLabel = "CommandDialysisAllowGuests".Translate(),
                defaultDesc = "CommandDialysisAllowGuestsDescription".Translate(),
                icon = CompDialysisMachine.ColonistOnlyCommandTex,
                isActive = () => this.allowGuests,
                toggleAction = delegate
                {
                    this.allowGuests = !this.allowGuests;
                },
                activateSound = SoundDefOf.Tick_Tiny
            };

        }
    }
}
