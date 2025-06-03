using RimWorld;
using System;
using Verse;

namespace RimKnights
{
    public class ThoughtWorker_Precept_IsOripathic : ThoughtWorker_Precept
    {
        protected override ThoughtState ShouldHaveThought(Pawn p)
        {
            return ThoughtWorker_Precept_IsOripathic.IsOripathic(p);
        }
        public static bool IsOripathic(Pawn p)
        {
            return p.health.hediffSet.GetFirstHediff<Hediff_Oripathy>() != null;
        }
    }

}
