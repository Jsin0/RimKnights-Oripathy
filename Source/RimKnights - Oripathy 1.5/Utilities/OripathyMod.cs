
using UnityEngine;
using Verse;

namespace RimKnights.Oripathy
{
    public class OripathyMod : Mod
    {
        internal static OripathyModSettings settings;

        public static bool baselinersImmune;

        public static bool infectionMonitor;

        public static bool abandonOripathicCorpses;

        public OripathyMod(ModContentPack content) : base(content)
        {
            settings = GetSettings<OripathyModSettings>();

            baselinersImmune = settings.baselinersImmune;
            infectionMonitor = settings.infectionMonitor;
            abandonOripathicCorpses = settings.abandonOripathicCorpses;
        }
        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listingStandard = new Listing_Standard();

            listingStandard.Begin(inRect);
            listingStandard.CheckboxLabeled("BaselinersImmuneLabel".Translate(), ref settings.baselinersImmune, "BaselinersImmuneDesc".Translate());
            listingStandard.CheckboxLabeled("InfectionMonitorLabel".Translate(), ref settings.infectionMonitor, "InfectionMonitorDesc".Translate());
            listingStandard.CheckboxLabeled("AbandonOripathicCorpsesLabel".Translate(), ref settings.abandonOripathicCorpses, "AbandonOripathicCorpsesDesc".Translate());
            listingStandard.End();
            base.DoSettingsWindowContents(inRect);
        }
        public override string SettingsCategory()
        {
            return "RimKnights - Oripathy";
        }
    }
}
