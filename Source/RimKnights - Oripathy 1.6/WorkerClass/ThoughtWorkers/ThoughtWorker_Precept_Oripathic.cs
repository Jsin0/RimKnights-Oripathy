using RimWorld;
using System;
using Verse;

namespace RimKnights.Oripathy
{
    public class ThoughtWorker_Precept_Oripathic : ThoughtWorker_Precept
    {
        protected override ThoughtState ShouldHaveThought(Pawn p)
        {
            if (!ModsConfig.IdeologyActive)
            {
                return false;
            }

            return ThoughtWorker_Oripathic.HasOripathyStage(p);
        }
    }

}
