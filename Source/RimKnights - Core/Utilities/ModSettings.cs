
using UnityEngine;
using Verse;

namespace RimKnights
{
    internal class CoreModSettings : ModSettings
    {
        public bool baselinersImmune = false;

        public bool originiumSpawnsPollution = true;

        public bool orifuel = false;

        public bool orundum = false;

        public bool infectionMonitor = false;
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref baselinersImmune, "baselinersImmune", false);
            Scribe_Values.Look(ref originiumSpawnsPollution, "originiumSpawnsPollution", true);
            Scribe_Values.Look(ref orifuel, "orifuel", false);
            Scribe_Values.Look(ref orundum, "orundum", false);
            Scribe_Values.Look(ref infectionMonitor, "infectionMonitor", false);
        }
    }

}
