using Verse;

namespace RimKnights.Oripathy
{
    public class HediffCompProperties_Harvestable : HediffCompProperties
    {
        public HediffCompProperties_Harvestable()
        {
            this.compClass = typeof(HediffComp_Harvestable);
        }

        public ThingDef resource;

        public int count = 1;

        public int cooldownHours = 96;

        public float miniumSeverity = 50f;

        public float severityOffset = 0f;
    }
}
