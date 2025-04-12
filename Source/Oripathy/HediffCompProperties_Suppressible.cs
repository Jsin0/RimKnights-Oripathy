using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace Originium
{
    public class HediffCompProperties_Suppressible : HediffCompProperties
    {
        public HediffCompProperties_Suppressible()
        {
            this.compClass = typeof(HediffComp_Suppressible);
        }

        public HediffDef suppressor = null;

        public HediffDef suppressedHediff = null;

        public HediffDef unsuppressedHediff = null;

        public int checkInterval = 250;
    }
}
