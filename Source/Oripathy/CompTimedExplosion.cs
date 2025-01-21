using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace Oripathy
{
    public class CompTimedExplosion : ThingComp
    {
        private int timer;
        public override void CompTick()
        {
            base.CompTick();
            if (timer <= 0)
            {
                Explode();
            }
            else
            {
                timer--;
            }
        }

        public override void Initialize(CompProperties props)
        {
            base.Initialize(props);
            timer = ((CompProperties_TimedExplosion)props).durationTicks;
        }
        private void Explode()
        {
            GenExplosion.DoExplosion(parent.Position,parent.Map,((CompProperties_TimedExplosion)props).explosionRadius,DamageDefOf.Bomb,parent,((CompProperties_TimedExplosion)props).damageAmount);
            parent.Destroy(DestroyMode.Vanish);
        }
    }
    public class CompProperties_TimedExplosion: CompProperties
    {
        public int durationTicks;
        public float explosionRadius;
        public int damageAmount;

        public CompProperties_TimedExplosion()
        {
            compClass = typeof(CompTimedExplosion);
        }
    }
}
