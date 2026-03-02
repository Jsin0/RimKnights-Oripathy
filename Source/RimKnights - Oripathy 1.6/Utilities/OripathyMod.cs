
using System;
using RimKnights.Originium;
using UnityEngine;
using Verse;

namespace RimKnights.Oripathy
{
    public class OripathyMod : Mod
    {
        internal static OripathyModSettings settings;

        public static readonly bool originiumModActive = ModsConfig.IsActive("RimKnights.Originium");

        public OripathyMod(ModContentPack content) : base(content)
        {
            settings = GetSettings<OripathyModSettings>();
        }
        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listingStandard = new Listing_Standard();

            listingStandard.Begin(inRect);
            listingStandard.CheckboxLabeled("BaselinersImmuneLabel".Translate(), ref settings.baselinersImmune, "BaselinersImmuneDesc".Translate());
            listingStandard.CheckboxLabeled("DebugModeLable".Translate(), ref settings.debugMode, "DebugModeDesc".Translate());
            listingStandard.CheckboxLabeled("InfectionMonitorLabel".Translate(), ref settings.infectionMonitor, "InfectionMonitorDesc".Translate());
            listingStandard.CheckboxLabeled("AbandonOripathicCorpsesLabel".Translate(), ref settings.abandonOripathicCorpses, "AbandonOripathicCorpsesDesc".Translate());
            settings.oripathyChance = (float)Math.Round(listingStandard.SliderLabeled($"{"GlobalOripathyChanceLabel".Translate()} : {settings.oripathyChance * 100}%", settings.oripathyChance, 0f, 1.0f, 0.3f, "GlobalOripathyChanceDesc".Translate()),2);

            if (listingStandard.ButtonText("ResetSettings".Translate(), null, 1f)) OripathyMod.settings.Reset();
            listingStandard.End();
            base.DoSettingsWindowContents(inRect);
        }
        public override string SettingsCategory()
        {
            return "RimKnights - Oripathy";
        }
    }
}
