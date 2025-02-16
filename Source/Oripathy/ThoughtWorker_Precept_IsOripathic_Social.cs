using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
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
