using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;

namespace Oripathy
{
    public class OripathicCorpse : Corpse
    {
        public CompForbiddable Forbiddable
        {
            get
            {
                CompForbiddable compForbiddable;
                if ((compForbiddable = this.compForbidInt) == null)
                {
                    compForbiddable = (this.compForbidInt = base.GetComp<CompForbiddable>());
                }
                return compForbiddable;
            }
        }
        public override void PostMake()
        {
            base.PostMake();
            this.Forbiddable.Forbidden = true;

        }
        public override void TickRare()
        {
            base.TickRare();
            Hediff_Oripathy firstHediff = this.InnerPawn.health.hediffSet.GetFirstHediff<Hediff_Oripathy>();
            firstHediff.TickRare();
            //check if shattering?
        }

        private CompForbiddable compForbidInt;
    }


}
