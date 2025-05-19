using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace Originium
{
    public class Building_OriginiumCluster : Mineable
    {
        public override void PostApplyDamage(DamageInfo dinfo, float totalDamageDealt)
        {
            base.PostApplyDamage(dinfo, totalDamageDealt);
            if(dinfo.Def == DamageDefOf.RK_ActiveOriginium && ready)
            {
                Spread();
                Grow();
            }
        }
        public override void TickRare()
        {
            base.TickRare();
            if (cooldownTicks > 0)
            {
                cooldownTicks -= 250;
            }
            else
            {
                ready = true;
            }
        }
        public void Spread()
        {

        }

        public void Grow()
        {

        }

        private bool ready = false;

        private int cooldownTicks = 60000;

        private List<ThingDef> stages = new List<ThingDef>();
    }
}
