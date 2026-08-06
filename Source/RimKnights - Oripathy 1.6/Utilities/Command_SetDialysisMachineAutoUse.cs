using System;
using System.Collections.Generic;
using RimWorld;
using UnityEditor;
using UnityEngine;
using Verse;

namespace RimKnights.Oripathy
{
    [StaticConstructorOnStartup]
    public class Command_SetDialysisMachineAutoUse : Command
    {
        private readonly CompDialysisMachine comp;
        private static Texture2D autoUseForEveryoneTex;
        private static Texture2D noAutoUseTex;
        private static Texture2D AutoUseForEveryoneTex
        {
            get
            {
                if(Command_SetDialysisMachineAutoUse.autoUseForEveryoneTex == null)
                {
                    Command_SetDialysisMachineAutoUse.autoUseForEveryoneTex = ContentFinder<Texture2D>.Get("UI/Gizmos/NeuralSupercharger_EveryoneAutoUse", true);
                }
                return Command_SetDialysisMachineAutoUse.autoUseForEveryoneTex;
            }
        }
        private static Texture2D NoAutoUseTex
        {
            get
            {

                if (Command_SetDialysisMachineAutoUse.noAutoUseTex == null)
                {
                    Command_SetDialysisMachineAutoUse.noAutoUseTex = ContentFinder<Texture2D>.Get("UI/Gizmos/NeuralSupercharger_NoAutoUse", true);
                }
                return Command_SetDialysisMachineAutoUse.noAutoUseTex;
            }
        }
        public Command_SetDialysisMachineAutoUse(CompDialysisMachine comp)
        {
            comp = comp;
            switch (comp.autoUseMode)
            {
                case CompDialysisMachine.AutoUseMode.NoAutoUse:
                    defaultLabel = "CommandDialysisNoAutoUse".Translate();
                    defaultDesc = "CommandDialysisNoAutoUseDesc".Translate();
                    icon = Command_SetDialysisMachineAutoUse.NoAutoUseTex;
                    return;
                case CompDialysisMachine.AutoUseMode.AutoUseForEveryone:
                    defaultLabel = "CommandDialysisAutoForEveryone".Translate();
                    defaultDesc = "CommandDialysisAutoForEveryoneDesc".Translate();
                    icon = Command_SetDialysisMachineAutoUse.AutoUseForEveryoneTex;
                    return;
                default:
                    Log.Error(string.Format("Unknown auto use mode: {0}", comp.autoUseMode));
                    return;
            }
        }
        public override void ProcessInput(Event ev)
        {
            base.ProcessInput(ev);
            List<FloatMenuOption> list = new List<FloatMenuOption>();
            list.Add(new FloatMenuOption("CommandDialysisMachineNoAutoUse".Translate(), delegate
            {
                this.comp.autoUseMode = CompDialysisMachine.AutoUseMode.NoAutoUse;
            }, Command_SetDialysisMachineAutoUse.NoAutoUseTex, Color.white, MenuOptionPriority.Default, null, null, 0f, null, null, true, 0, HorizontalJustification.Left, false));
            list.Add(new FloatMenuOption("CommandDialysisMachineAutoForEveryone".Translate(), delegate
            {
                this.comp.autoUseMode = CompDialysisMachine.AutoUseMode.AutoUseForEveryone;
            }, Command_SetDialysisMachineAutoUse.AutoUseForEveryoneTex, Color.white, MenuOptionPriority.Default, null, null, 0f, null, null, true, 0, HorizontalJustification.Left, false));
            Find.WindowStack.Add(new FloatMenu(list));
        }
    }
}
