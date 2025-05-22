using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using Verse.Noise;

namespace Originium
{
    public class CompMutable : ThingComp
    {
        public CompProperties_Mutable Props
        {
            get
            {
                return (CompProperties_Mutable)this.props;
            }
        }
        public override void Initialize(CompProperties props)
        {
            base.Initialize(props);
            ResetCooldown();
        }
        private void ResetCooldown()
        {
            this.cooldownTicksLeft = UnityEngine.Mathf.RoundToInt(Props.cooldownHours * 2500f);
            this.ready = false;
        }
        public override void PostPostApplyDamage(DamageInfo dinfo, float totalDamageDealt)
        {
            if (dinfo.Def == Props.trigger && ready) 
            {
                if (Rand.Chance(Props.chance))
                {
                    TrySpread();
                    TryGrow();
                }
            }
        }
        public override void CompTick()
        {
            if (!ready)
                {
                if (this.parent.IsHashIntervalTick(250))
                {
                    if (this.cooldownTicksLeft > 0) 
                    { 
                        this.cooldownTicksLeft -= 250;
                    }
                    else
                    {
                        ready = true;
                    }
                }
            }
            else
            {
                if(this.parent.IsHashIntervalTick(UnityEngine.Mathf.RoundToInt(Props.spreadIntervalHours * 2500f)))
                {

                }
            }
        }
        private void TrySpread()
        {
            if (Props.offspring != null)
            {
                if (Rand.Chance(Props.chance))
                {
                    GenExplosion.DoExplosion(this.parent.Position, this.parent.Map, Props.effectRadius, DamageDefOf.RK_ActiveOriginium, this.parent, -1, -1, null, null, null, null, Props.offspring, Props.chance);
                    ResetCooldown();
                }
            }
        }
        private void TryGrow()
        {
            if (Props.changeInto == null || this.parent == null) return;

            if (Rand.Chance(Props.chance))
            {
                if (Props.changeInto.size.x > 1 || Props.changeInto.size.z > 1)
                {
                    Merge();
                }else
                {
                    GenSpawn.Spawn(Props.changeInto, this.parent.Position, this.parent.Map);
                }

            }

        }
        private void Merge()
        {
            IntVec3 location = this.parent.Position;
            Map map = this.parent.Map;
            IntVec2 thingSize = Props.changeInto.size;

            List<IntVec3> clusterOffsets = new List<IntVec3>();

            //Adds the cells to check depending on how big the thing will be
            for (int i = 0; i < thingSize.x; i++)
            {
                for (int j = 0; j < thingSize.z; j++)
                {
                    clusterOffsets.Add(new IntVec3(i, 0, j));
                }
            }

            List<Thing> mergeableThings = new List<Thing>();
            IntVec3 finalOffset = new IntVec3 { };
            IntVec3 newLocation = new IntVec3 { };

            foreach (IntVec3 cluster in clusterOffsets)
            {
                newLocation = location - cluster;

                mergeableThings.Clear();

                Thing found;
                foreach (IntVec3 offset in clusterOffsets)
                {
                    IntVec3 checkCell = newLocation + offset;

                    found = checkCell.GetFirstThing(map, this.parent.def);
                    //Log.Message(this.parent.def + " found in cell");
                    if (!checkCell.InBounds(map) && found == null)
                    {
                        //Log.Message(this.parent.def + " not found in cell. terminating attempt");
                        mergeableThings.Clear();
                        break;
                    }

                    //Log.Message(found.Label +  " | " + found.Position);
                    mergeableThings.Add(found);
                }

                if (mergeableThings.Count > 0)
                {
                    break;
                }
            }

            if (mergeableThings.Count == thingSize.Area)
            {
                CellRect placementRect = new CellRect(newLocation.x, newLocation.z, thingSize.x, thingSize.z);
                if (!placementRect.InBounds(map)) return;

                /*
                foreach (IntVec3 placementCell in placementRect)
                {
                    if (!placementCell.Standable(map)) 
                    { 
                        Log.Message("placementCell (" +  placementCell.x + "," + placementCell.z + ") not standable");
                        return; 
                    }
                }
                */
                GenSpawn.Spawn(Props.changeInto, newLocation, map);
                ResetCooldown();
            }
        }

        private int cooldownTicksLeft;

        private bool ready = false;

    }
}
