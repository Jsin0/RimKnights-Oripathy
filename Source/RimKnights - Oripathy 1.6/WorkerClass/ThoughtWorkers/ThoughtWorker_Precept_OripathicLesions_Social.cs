using RimWorld;
using System;
using Verse;

namespace RimKnights.Oripathy
{
    public class ThoughtWorker_Precept_OripathicLesions_Social : ThoughtWorker_Precept_Social
    {
        protected override ThoughtState ShouldHaveThought(Pawn p, Pawn otherPawn)
        {
            return ThoughtWorker_Precept_OripathicLesions.HasLesions(otherPawn);
        }
    }
}
