using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;

namespace RimKnights.Oripathy
{
    public class RitualObligationTrigger_Infection : RitualObligationTrigger_EveryMember
    {

        public override string TriggerExtraDesc
        {
            get
            {
                return "RitualOripathyTriggerExtraDesc".Translate(this.ritual.ideo.memberName.Named("IDEOMEMBER"), this.ritual.ideo.MemberNamePlural.Named("IDEOMEMBERPLURAL"));
            }
        }
        protected override void Recache()
        {
            try
            {
                if (this.ritual.activeObligations != null)
                {
                    this.ritual.activeObligations.RemoveAll(delegate (RitualObligation o)
                    {
                        Pawn pawn2 = o.targetA.Thing as Pawn;
                        return pawn2 != null && pawn2.Ideo != this.ritual.ideo;
                    });
                    foreach (RitualObligation ritualObligation in this.ritual.activeObligations)
                    {
                        RitualObligationTrigger_Infection.existingObligations.Add(ritualObligation.targetA.Thing as Pawn);
                    }
                }
                foreach (Pawn pawn in PawnsFinder.AllMapsCaravansAndTravelingTransportPods_Alive_Colonists)
                {
                    if (!RitualObligationTrigger_Infection.existingObligations.Contains(pawn) && pawn.Ideo != null && pawn.Ideo == this.ritual.ideo && !pawn.IsPrisoner)
                    {
                        if (!pawn.health.hediffSet.HasHediff(HediffDefOf.RK_Oripathy, true))
                        {
                            this.ritual.AddObligation(new RitualObligation(this.ritual, pawn, false));
                        }
                    }
                }
            }
            finally
            {
                RitualObligationTrigger_Infection.existingObligations.Clear();
            }
        }

        private static List<Pawn> existingObligations = new List<Pawn>();
    }
}
