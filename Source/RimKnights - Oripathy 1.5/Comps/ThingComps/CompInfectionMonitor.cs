using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimKnights.Oripathy
{
    public class CompInfectionMonitor : ThingComp
    {
        public CompProperties_InfectionMonitor Props
        {
            get
            {
                return (CompProperties_InfectionMonitor)this.props;
            }
        }

        public override void Notify_Equipped(Pawn pawn)
        {
            if (!pawn.health?.hediffSet.HasHediff(Core.HediffDefOf.RK_InfectionMonitorImplant) ?? false)
            {
                pawn.health.AddHediff(Core.HediffDefOf.RK_InfectionMonitorWorn);
            }
        }
        public override void Notify_Unequipped(Pawn pawn)
        {
            Hediff hediff = pawn.health?.hediffSet.GetFirstHediffOfDef(Core.HediffDefOf.RK_InfectionMonitorWorn);
            if (hediff != null) pawn.health.RemoveHediff(hediff);
        }
    }
}
