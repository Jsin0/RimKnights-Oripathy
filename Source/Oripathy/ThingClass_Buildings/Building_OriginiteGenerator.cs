using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using Verse.Noise;

namespace RimKnights
{
    public class Building_OriginiteGenerator : Building
    {
        public override void Tick()
        {
            base.Tick();
            if (this.IsHashIntervalTick(250))
            {
                if (this.IsBrokenDown())
                {
                    if (Rand.MTBEventOccurs(8f, 2500f, 250))
                    {
                        if (ModsConfig.BiotechActive && OriMod.settings.originiumSpawnsPollution)
                        {
                            PollutionUtility.GrowPollutionAt(this.Position, this.Map, 1);
                        }
                        if(Rand.Chance(0.20f)) GenExplosion.DoExplosion(this.Position, this.Map, Math.Max(this.def.size.x,this.def.size.z), DamageDefOf.RK_ActiveOriginium, null, 100, 0, null, null, null, null, ThingDefOf.RK_OriginiumCluster, 0.1f, 1, null, false, null,0, 1, 0.1f, true, null, null, null, false, 1, 0, false, null, 0, null, null);
                    }

                }
                /*
                else
                {
                    if (Rand.Chance(0.05f))
                    {
                        GenExplosion.DoExplosion(this.Position, this.Map, Math.Max(this.def.size.x, this.def.size.z), DamageDefOf.RK_ActiveOriginium, null, 25, 0, null, null, null, null, null, 0, 1, null, false, null, 0, 1, 0.1f, true, null, null, null, false, 1, 0, false, null, 0, null, null);
                    }

                }*/
            }
        }
    }
}
