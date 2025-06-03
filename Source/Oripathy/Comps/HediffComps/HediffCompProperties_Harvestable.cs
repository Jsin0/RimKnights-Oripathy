using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Originium
{
    public class HediffCompProperties_Harvestable : HediffCompProperties
    {
        public HediffCompProperties_Harvestable()
        {
            this.compClass = typeof(HediffComp_Harvestable);
        }

        public ThingDef resource;

        public int cooldownHours = 96;

        public float miniumSeverity = 50f;

        public float severityOffset = 0f;
    }
}
