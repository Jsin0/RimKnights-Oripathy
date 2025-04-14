using RimWorld;
using System;
using Verse;

namespace Originium
{
    public class ThoughtWorker_Precept_IsOripathic_Social : ThoughtWorker_Precept_Social
    {
        protected override ThoughtState ShouldHaveThought(Pawn p, Pawn otherPawn)
        {
            return ThoughtWorker_Precept_IsOripathic.IsOripathic(otherPawn);
        }
    }
}
