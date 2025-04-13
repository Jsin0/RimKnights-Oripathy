using System.Collections.Generic;
using Verse;
using UnityEngine;

namespace Originium
{
    public class OripathyModSettings : ModSettings
    {
        public bool baselinersImmune = false;

        public override void ExposeData()
        {
            Scribe_Values.Look(ref baselinersImmune, "Baseliners are immune to oripathy.");
            base.ExposeData();
        }
    }

    public class OripathyMod : Mod
    {
        OripathyModSettings settings;
        public OripathyMod(ModContentPack content) : base(content) 
        {
            this.settings = GetSettings<OripathyModSettings>();
        }
        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listingStandard = new Listing_Standard();

            listingStandard.Begin(inRect);
            listingStandard.CheckboxLabeled("Baseliners are immune", ref settings.baselinersImmune, "Check to make baseliners immune to oripathy. Other xenotypes will be unnaffected.");
            listingStandard.End();
            base.DoSettingsWindowContents(inRect);
        }
        public override string SettingsCategory()
        {
            return "RimKnights - Oripathy".Translate();
        }
    }
}
