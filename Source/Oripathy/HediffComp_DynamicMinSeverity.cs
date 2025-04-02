using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace Originium
{
    public class HediffComp_DynamicSeverityRange : HediffComp
    {
        private HediffCompProperties_DynamicSeverityRange Props
        {
            get
            {
                return (HediffCompProperties_DynamicSeverityRange)this.props;
            }
        }

        public override void CompPostPostAdd(DamageInfo? dinfo)
        {
            base.CompPostPostAdd(dinfo);
            CalculateSeverity();
        }

        public void CalculateSeverity(bool isMin = true)
        {
            Hediff hediff = this.Pawn.health.hediffSet.GetFirstHediffOfDef(this.Props.hediff);
            float severity = ((hediff != null) ? hediff.Severity : 0f);

            float newLimit = ((isMin) ? this.Props.CalculateMinSeverity(severity): this.Props.CalculateMaxSeverity(severity));

            if (newLimit == -8)
            {
                return;
            }

            if (isMin)
            {
                this.Def.minSeverity = newLimit;
            }
            else
            {
                this.Def.maxSeverity = newLimit;
            }

        }


    }
}
