
using UnityEngine;
using Verse;

namespace Originium
{
    public class OripathyMod : Mod
    {
        internal static OripathyModSettings settings;
        public OripathyMod(ModContentPack content) : base(content)
        {
            settings = GetSettings<OripathyModSettings>();
        }
        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listingStandard = new Listing_Standard();

            listingStandard.Begin(inRect);
            listingStandard.CheckboxLabeled("Baseliners are immune", ref settings.baselinersImmune, "Check to make baseliners immune to oripathy. Other xenotypes will be unnaffected.");
            listingStandard.CheckboxLabeled("Originium clusters spread pollution", ref settings.originiumSpawnsPollution, "Check to make originium crystals pollute the ground when they spawn.");
            listingStandard.End();
            base.DoSettingsWindowContents(inRect);
        }
        public override string SettingsCategory()
        {
            return "RimKnights - Oripathy";
        }
    }
}
