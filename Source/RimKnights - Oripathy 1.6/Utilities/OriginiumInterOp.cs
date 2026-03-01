using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimKnights.Oripathy
{
    public static class OriginiumInterOp
    {
        public static ThingDef GetClusterDef()
        {
            return RimKnights.Originium.ThingDefOf.RK_OriginiumCluster;
        }

        public static void SpawnCluster(IntVec3 pos, Map map)
        {
            GenSpawn.Spawn(
            RimKnights.Originium.ThingDefOf.RK_OriginiumCluster,
            pos, map, WipeMode.FullRefund);
        }
    }
}
