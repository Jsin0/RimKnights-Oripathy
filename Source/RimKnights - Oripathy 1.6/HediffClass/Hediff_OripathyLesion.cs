using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimKnights.Oripathy
{
    internal class Hediff_OripathyLesion : HediffWithComps
    {
        public override void PostAdd(DamageInfo? dinfo)
        {
            base.PostAdd(dinfo);
            if(this.Part.depth == BodyPartDepth.Outside)
            {
                pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.RK_Oripathy)?.SetVisible();
                this.SetVisible();
            }
        }
    }
}
