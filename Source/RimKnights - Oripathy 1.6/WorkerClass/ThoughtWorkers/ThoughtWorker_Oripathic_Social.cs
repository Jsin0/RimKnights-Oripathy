using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimKnights.Oripathy
{
    public class ThoughtWorker_Oripathic_Social : ThoughtWorker
    {
        protected override ThoughtState CurrentSocialStateInternal(Pawn p, Pawn otherPawn)
        {
            return ThoughtWorker_Oripathic.HasOripathyStage(otherPawn).StageIndex > 0;
        }
    }
}
