using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace Oripathy
{
    public class ThoughtWorker_Oripathic_Solidarity : ThoughtWorker
    {
        protected override ThoughtState CurrentSocialStateInternal(Pawn pawn, Pawn other)
        {
            if (pawn == null || pawn.Ideo == null)
            {
                return false;
            }
            if (!pawn.RaceProps.Humanlike)
            {
                return false;
            }
            if (!other.RaceProps.Humanlike || other.Dead)
            {
                return false;
            }
            if (!RelationsUtility.PawnsKnowEachOther(pawn, other))
            {
                return false;
            }
            if (pawn.health.hediffSet.GetFirstHediff<Hediff_Oripathy>() == null)
            {
                return false;
            }
            return other.health.hediffSet.GetFirstHediff<Hediff_Oripathy>() != null;
        }
    }
}
