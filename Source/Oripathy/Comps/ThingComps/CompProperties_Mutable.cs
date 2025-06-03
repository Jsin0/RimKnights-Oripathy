using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace Originium
{
    public class CompProperties_Mutable : CompProperties
    {
        public CompProperties_Mutable() 
        { 
            this.compClass = typeof(CompMutable); 
        }

        public DamageDef trigger;

        public ThingDef changeInto;

        public ThingDef offspring;

        public float cooldownHours = 1f;

        public float spreadMTBHours = 12f;

        public int spreadCount = 8;
    }
}
