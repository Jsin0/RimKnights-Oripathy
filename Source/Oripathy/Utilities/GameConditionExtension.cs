using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace Originium.Utilities
{
    public class GameConditionExtension : DefModExtension
    {
        public bool protectedByRoof = true;

        public bool protectedIndoors = false;

        public float damageMultiplier = 1f;

        public int damageInterval = 3451;

        public float compMutableCooldownFactor;
    }

    public class BuildingExtension : DefModExtension
    {
        public int cooldownTicks = 60000;
    }
}
