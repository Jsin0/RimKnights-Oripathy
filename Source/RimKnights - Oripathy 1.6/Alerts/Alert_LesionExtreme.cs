using RimWorld;
using Verse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RimKnights.Oripathy
{
    public class Alert_LesionExtreme : Alert_Critical
    {
        private List<Pawn> OripathicPawns
        {
            get
            {
                this.oripathicPawnsResult.Clear();
                foreach (Pawn pawn in PawnsFinder.AllMapsCaravansAndTravellingTransporters_Alive_FreeColonistsAndPrisoners_NoCryptosleep)
                {
                    for(int i = 0; i < pawn.health.hediffSet.hediffs.Count; i++)
                    {
                        Hediff hediff = pawn.health.hediffSet.hediffs[i];
                        if(hediff.def == HediffDefOf.RK_OripathyLesion && hediff.Severity > 0.8f)
                        {
                            this.oripathicPawnsResult.Add(pawn);
                            break;
                        }
                    }
                }
                return this.oripathicPawnsResult;
            }
        }
        public override string GetLabel()
        {
            return "AlertOripathicLesion".Translate();
        }
        public override TaggedString GetExplanation()
        {
            StringBuilder stringBuilder = new StringBuilder();
            bool amputatable = false;
            foreach (Pawn pawn in this.OripathicPawns)
            {
                stringBuilder.AppendLine("  - " + pawn.NameShortColored.Resolve());
                foreach(Hediff hediff in pawn.health.hediffSet.hediffs)
                {
                    if (hediff.def == HediffDefOf.RK_OripathyLesion && hediff.Severity > 0.8f && hediff.Part != null && hediff.Part != pawn.RaceProps.body.corePart && (pawn.Ideo == null || !pawn.Ideo.HasPrecept(PreceptDefOf.Oripathy_Required) || !pawn.Ideo.HasPrecept(PreceptDefOf.Oripathy_Exalted)))
                    {
                        amputatable = true;
                        break;
                    }
                }
            }

            if (amputatable)
            {
                return "AlertOripathicLesionAmputationDesc".Translate(stringBuilder.ToString().TrimEndNewlines());
            }

            return "AlertOripathicLesionDesc".Translate(stringBuilder.ToString().TrimEndNewlines());

        }
        public override AlertReport GetReport()
        {
            return AlertReport.CulpritsAre(this.OripathicPawns);
        }
        private List<Pawn> oripathicPawnsResult = new List<Pawn>();
    }
}
