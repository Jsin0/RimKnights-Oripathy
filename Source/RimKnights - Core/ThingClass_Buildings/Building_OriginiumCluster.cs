using RimWorld;
using Verse;

namespace RimKnights
{
    public class Building_OriginiumCluster : Mineable
    {
        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            if (ModsConfig.BiotechActive && CoreMod.originiumSpawnsPollution)
            {
                PollutionUtility.GrowPollutionAt(this.Position,map,1);
            }
        }
    }
}
