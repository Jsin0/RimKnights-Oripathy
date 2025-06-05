using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace RimKnights
{
    public class CompProperties_HazardTouch : CompProperties
    {
        public CompProperties_HazardTouch() 
        { 
            this.compClass = typeof(CompHazardTouch); 
        }

        public DamageDef damageDef;

        public float baseSpringChance = 1f;

        public float baseDamage = 25f;

        public float trapPeacefulWildAnimalsSpringChanceFactor = 1f;

        public float numOfAttacks = 1f;
    }
}
