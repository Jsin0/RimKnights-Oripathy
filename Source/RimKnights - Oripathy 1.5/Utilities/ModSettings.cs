
using UnityEngine;
using Verse;

namespace RimKnights.Oripathy
{
    public class OripathyModSettings : ModSettings
    {
        public bool baselinersImmune = false;

        public bool infectionMonitor = false;

        public bool abandonOripathicCorpses = false;
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref baselinersImmune, "baselinersImmune", false);
            Scribe_Values.Look(ref infectionMonitor, "infectionMonitor", false);
            Scribe_Values.Look(ref abandonOripathicCorpses, "abandonOripathicCorpses", false);
        }
    }

}
