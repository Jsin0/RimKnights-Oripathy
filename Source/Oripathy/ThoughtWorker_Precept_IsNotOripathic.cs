using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace Oripathy
{
    public class ThoughtWorker_Precept_IsNotOripathic : ThoughtWorker_Precept
    {
        protected override ThoughtState ShouldHaveThought(Pawn p)
        {
            return !ThoughtWorker_Precept_IsOripathic.IsOripathic(p);
        }
    }
}
