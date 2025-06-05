using RimWorld;
using System;
using Verse;

namespace RimKnights
{
    public class ThoughtWorker_Precept_IsNotOripathic : ThoughtWorker_Precept
    {
        protected override ThoughtState ShouldHaveThought(Pawn p)
        {
            return !ThoughtWorker_Precept_IsOripathic.IsOripathic(p);
        }
    }
}
