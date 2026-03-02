
using JetBrains.Annotations;
using UnityEngine;
using Verse;

namespace RimKnights.Oripathy
{
    public class OripathyModSettings : ModSettings
    {
        public bool baselinersImmune;
        public bool infectionMonitor;
        public bool abandonOripathicCorpses;
        public bool debugMode;
        public float oripathyChance;

        public const bool DefaultBaselinersImmune = false;
        public const bool DefaultInfectionMonitor = true;
        public const bool DefaultAbandonOripathicCorpses = true;
        public const bool DefaultDebugMode = false;
        public const float DefaultOripathyChance = 0.05f;
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref baselinersImmune, "baselinersImmune", DefaultBaselinersImmune);
            Scribe_Values.Look(ref infectionMonitor, "infectionMonitor", DefaultInfectionMonitor);
            Scribe_Values.Look(ref abandonOripathicCorpses, "abandonOripathicCorpses", DefaultAbandonOripathicCorpses);
            Scribe_Values.Look(ref debugMode, "debugMode", DefaultDebugMode);
            Scribe_Values.Look(ref oripathyChance, "oripathyChance", DefaultOripathyChance);
        }

        public void Reset()
        {
            baselinersImmune = DefaultBaselinersImmune;
            infectionMonitor = DefaultInfectionMonitor;
            abandonOripathicCorpses = DefaultAbandonOripathicCorpses;
            debugMode = DefaultDebugMode;
            oripathyChance = DefaultOripathyChance;
        }
    }

}
