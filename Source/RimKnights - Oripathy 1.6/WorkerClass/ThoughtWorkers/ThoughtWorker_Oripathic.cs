using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimKnights.Oripathy
{
    public class ThoughtWorker_Oripathic : ThoughtWorker
    {
        public static ThoughtState CurrentThoughtState(Pawn p)
        {
            return ThoughtWorker_Oripathic.HasOripathyStage(p);
        }
        protected override ThoughtState CurrentStateInternal(Pawn p)
        {
            return ThoughtWorker_Oripathic.HasOripathyStage(p).StageIndex > 0;
        }
        public static ThoughtState HasOripathyStage(Pawn p)
        {
            if (p.Faction == null)
            {
                return false;
            }
            Hediff_Oripathy hediff = p.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.RK_Oripathy, true) as Hediff_Oripathy;
            float severity = 0f;
            if (hediff != null)
            {
                severity = OripathyMod.infectionMonitor ? hediff.displayedSeverity : hediff.Severity;
            }
            if (Core.CoreMod.settings.debugMode && Core.CoreMod.settings.verboseLogging) { Log.Message($"[RimKnights - Oripathy] {p}'s displayedSeverity = {severity} "); }
            if (severity > 0.50f)
            {
                return ThoughtState.ActiveAtStage(3);
            }
            if (severity > 0.25f)
            {
                return ThoughtState.ActiveAtStage(2);
            }
            if (severity > 0f)
            {
                return ThoughtState.ActiveAtStage(1);
            }
            return ThoughtState.ActiveAtStage(0);
        }
    }
}
