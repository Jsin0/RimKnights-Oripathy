using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace Originium.Utilities
{
    public class OriginiumModExtension : DefModExtension
    {
        public bool protectedByRoof = true;

        public bool protectedIndoors = false;

        public float damageMultiplier = 1f;

        public int damageInterval = 3451;
    }
}
