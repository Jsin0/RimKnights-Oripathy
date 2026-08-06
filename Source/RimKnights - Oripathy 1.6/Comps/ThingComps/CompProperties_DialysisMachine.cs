using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimKnights.Oripathy
{
    public class CompProperties_DialysisMachine : CompProperties
    {
        public CompProperties_DialysisMachine() { this.compClass = typeof(CompDialysisMachine); }

        [MustTranslate]
        public string jobString;

        public List<HediffDef> hediffs;
        public float severityReductionPerHour = 0.125f;
        public bool shareSeverityReduction = true;
    }
}
