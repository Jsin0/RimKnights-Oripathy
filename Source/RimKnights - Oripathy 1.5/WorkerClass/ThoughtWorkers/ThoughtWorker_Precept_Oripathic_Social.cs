using RimWorld;
using System;
using Verse;

namespace RimKnights.Oripathy
{
    public class ThoughtWorker_Precept_Oripathic_Social : ThoughtWorker_Precept_Social
    {
        protected override ThoughtState ShouldHaveThought(Pawn p, Pawn otherPawn)
        {
            if (!ModsConfig.IdeologyActive)
            {
                return false;
            }
            return ThoughtWorker_Oripathic.HasOripathyStage(otherPawn);
        }
    }
}
