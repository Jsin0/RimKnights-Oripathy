using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;

namespace RimKnights
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
            foreach (Hediff hediff in pawn.health.hediffSet.hediffs.Where(hed => hed.GetType().IsSubclassOf(typeof(HediffVisibleWithApparel))))
            {
                hediff.SetVisible();
            }
        }
    }
}
