using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;

namespace RimKnights.Oripathy
{
    public class CompProperties_InfectionMonitor : CompProperties
    {
        public CompProperties_InfectionMonitor()
        {
            this.compClass = typeof(CompInfectionMonitor);
        }

    }
}
