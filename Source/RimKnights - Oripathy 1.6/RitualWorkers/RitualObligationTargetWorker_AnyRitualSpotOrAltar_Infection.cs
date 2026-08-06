using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace RimKnights.Oripathy
{
    public class RitualObligationTargetWorker_AnyRitualSpotOrAltar_Infection : RitualObligationTargetWorker_AnyRitualSpotOrAltar
    {
        public RitualObligationTargetWorker_AnyRitualSpotOrAltar_Infection() { }

        public RitualObligationTargetWorker_AnyRitualSpotOrAltar_Infection(RitualObligationTargetFilterDef def) : base(def) { }

        public override bool ObligationTargetsValid(RitualObligation obligation)
        {
            Pawn pawn;
            if((pawn = obligation.targetA.Thing as Pawn) == null)
            {
                return false;
            }
            if (pawn.Dead)
            {
                return false;
            }
            return pawn.Ideo != null && !pawn.health.hediffSet.HasHediff(HediffDefOf.RK_Oripathy, true);
        }
        public override string LabelExtraPart(RitualObligation obligation)
        {
            return obligation.targetA.Thing.LabelShort;
        }
    }
}
