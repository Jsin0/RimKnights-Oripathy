using RimWorld;
using Verse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Originium
{
    public class Recipe_BloodPurge : Recipe_BloodTransfusion
    {
        public override bool CompletableEver(Pawn surgeryTarget)
        {
            return base.CompletableEver(surgeryTarget) || surgeryTarget.health.hediffSet.HasHediff(HediffDefOf.RK_OriginiumBuildup);
        }

        public override bool AvailableOnNow(Thing thing, BodyPartRecord part = null)
        {
            return base.AvailableOnNow(thing, part);
        }
    }
}
