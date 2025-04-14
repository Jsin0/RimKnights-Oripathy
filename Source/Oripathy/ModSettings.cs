
using UnityEngine;
using Verse;

namespace Originium
{
    internal class OripathyModSettings : ModSettings
    {
        public bool baselinersImmune = false;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref baselinersImmune, "BaselinersImmune");
        }
    }

}
