
using UnityEngine;
using Verse;

namespace Originium
{
    public class OriMod : Mod
    {
        internal static OripathyModSettings settings;
        public OriMod(ModContentPack content) : base(content)
        {
            settings = GetSettings<OripathyModSettings>();
        }
        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listingStandard = new Listing_Standard();

            listingStandard.Begin(inRect);
            listingStandard.CheckboxLabeled("Baseliners are immune", ref settings.baselinersImmune, "Originium will never harm you, and deep down, you realize this once again.");
            listingStandard.CheckboxLabeled("Originium clusters spread pollution", ref settings.originiumSpawnsPollution, "Check to make originium crystals pollute the ground when they spawn.");
            listingStandard.CheckboxLabeled("Chemfuel is now orifuel", ref settings.orifuel, "Replaces 'chemfuel' from all text with 'orifuel' (defName is unchanged)");
            listingStandard.CheckboxLabeled("Silver is now orundum", ref settings.orundum, "Replaces 'silver' from all text with 'orundum' and replaces the texture for silver");
            listingStandard.End();
            base.DoSettingsWindowContents(inRect);
        }
        public override string SettingsCategory()
        {
            return "RimKnights - Oripathy";
        }
    }
}
