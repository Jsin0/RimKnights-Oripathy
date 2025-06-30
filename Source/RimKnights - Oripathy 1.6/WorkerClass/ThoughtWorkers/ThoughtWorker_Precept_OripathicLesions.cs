using RimWorld;
using System;
using Verse;

namespace RimKnights.Oripathy
{
    public class ThoughtWorker_Precept_OripathicLesions : ThoughtWorker_Precept
    {
        protected override ThoughtState ShouldHaveThought(Pawn p)
        {
            return HasLesions(p);
        }
        public static ThoughtState HasLesions(Pawn p)
        {
            if (!ModsConfig.IdeologyActive || p.Faction == null || p.health?.hediffSet.GetFirstHediffOfDef(HediffDefOf.RK_Oripathy) == null)
            {
                return false;
            }

            int hediffCount = p.health.hediffSet.GetHediffCount(HediffDefOf.RK_OripathyLesion);

            if (hediffCount > 4)
            {
                return ThoughtState.ActiveAtStage(3);
            }
            if (hediffCount > 2)
            {
                return ThoughtState.ActiveAtStage(2);
            }
            if(hediffCount > 0)
            {
                return ThoughtState.ActiveAtStage(1);
            }
            return ThoughtState.ActiveAtStage(0);
        }
    }
}
