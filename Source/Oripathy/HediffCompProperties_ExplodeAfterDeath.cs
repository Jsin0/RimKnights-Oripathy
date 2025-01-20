using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Oripathy
{
    public class HediffCompProperties_ExplodeAfterDeath : HediffCompProperties
    {
        public HediffCompProperties_ExplodeAfterDeath()
        {
            this.compClass = typeof(HediffComp_ExplodeAfterDeath);
        }

        public bool destroyGear;

        public bool destroyBody;

        public float explosionRadius;

        public DamageDef damageDef;

        public int damageAmount = -1;

        public int delay = 0;
    }
}
