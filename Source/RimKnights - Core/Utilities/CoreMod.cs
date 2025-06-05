
using UnityEngine;
using Verse;

namespace RimKnights
{
    public class CoreMod : Mod
    {
        internal static CoreModSettings settings;

        public static bool baselinersImmune;

        public static bool originiumSpawnsPollution;

        public static bool infectionMonitor;

        public static bool orifuel;

        public static bool orundum;
        public CoreMod(ModContentPack content) : base(content)
        {
            settings = GetSettings<CoreModSettings>();

            baselinersImmune = settings.baselinersImmune;
            originiumSpawnsPollution = settings.originiumSpawnsPollution;
            infectionMonitor = settings.infectionMonitor;
            orifuel = settings.orifuel;
            orundum = settings.orundum;
        }
        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listingStandard = new Listing_Standard();

            listingStandard.Begin(inRect);
            listingStandard.CheckboxLabeled("BaselinersImmuneLabel".Translate(), ref settings.baselinersImmune, "BaselinersImmuneDesc".Translate());
            listingStandard.CheckboxLabeled("OriginiumPollutionLabel".Translate(), ref settings.originiumSpawnsPollution, "OriginiumPollutionDesc".Translate());
            listingStandard.CheckboxLabeled("OrifuelLabel".Translate(), ref settings.orifuel, "OrifuelDesc".Translate());
            listingStandard.CheckboxLabeled("OrundumLabel".Translate(), ref settings.orundum, "OrundumDesc".Translate());
            listingStandard.CheckboxLabeled("InfectionMonitorLabel".Translate(), ref settings.infectionMonitor, "InfectionMonitorDesc".Translate());
            listingStandard.End();
            base.DoSettingsWindowContents(inRect);
        }
        public override string SettingsCategory()
        {
            return "RimKnights - Core";
        }
    }
}
