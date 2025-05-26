using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using Verse.Noise;
using UnityEngine;

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
        private int spreadIntervalTicks
        {
            get
            {
                return Mathf.Abs(Mathf.RoundToInt(this.Props.spreadIntervalHours * 2500f));
            }
        }
        private int cooldownTicks
        {
            get
            {
                return Mathf.Abs(Mathf.RoundToInt(this.Props.cooldownHours * 2500f));
            }
        }
        private void ResetCooldown()
        {
            if (this.active)
            {
                this.cooldownTicksLeft = cooldownTicks;
                this.ready = false;
            }
        }
        /*
        public override void PostPostApplyDamage(DamageInfo dinfo, float totalDamageDealt)
        {
            if (dinfo.Def == this.Props.trigger && ready) 
            {
                //if (Rand.Chance(this.Props.chance))
                {
                    TryGrow();
                    TrySpread();
                    ResetCooldown();
                }
            }
        }*/
        public override void CompTick()
        {
            base.CompTick();

            if (active && this.parent.IsHashIntervalTick(250))
            {
                CompTickRare();
            }

            if (this.parent.IsHashIntervalTick(1000))
            {
                CompTickLong();
            }

        }
        public override void CompTickRare()
        {
            base.CompTickRare();
            if (!ready)
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
            else
            {
                TryGrow();
                if (this.active)
                {
                    TrySpread();
                    ResetCooldown();
                }
            }
        }
        public override void CompTickLong()
        {
            base.CompTickLong();
            CheckActivity();
        }
        private void TrySpread()
        {
            if (this.Props.offspring != null && this.parent != null && active)
            {
                //Log.Message("spreading at " + this.parent.Position);
                Spread();
            }
        }
        private void Spread()
        {
            List<IntVec3> adjCells = GenAdjFast.AdjacentCells8Way(this.parent);
            Map map = this.parent.MapHeld;
            if (map == null) { return; }
            int count = 0;
            foreach (IntVec3 cell in adjCells)
            {
                if (!cell.InBounds(map) || cell == this.parent.PositionHeld) 
                { 
                    count++;
                    continue; 
                }

                Building building = cell.GetFirstBuilding(map);
                if (building != null)
                {
                    //Log.Message(building.GetType().ToString());
                    if (building.def == this.Props.offspring || building.GetType() == typeof(Building_OriginiumCluster))
                    {
                        count++;
                        DamageInfo dinfo = new DamageInfo(DamageDefOf.RK_ActiveOriginium, 1f);
                        building.TakeDamage(dinfo);
                        continue;
                    }
                }

                if (Rand.Chance(this.Props.chance))
                {
                    if (!cell.Walkable(map))
                    {
                        //Log.Message("damaging building");
                        DamageInfo dinfo = new DamageInfo(RimWorld.DamageDefOf.Stab, 100f);
                        building.TakeDamage(dinfo);
                        if (!building.Destroyed) continue;
                    }

                    //Log.Message("spawning thing at " + cell.ToString());
                    GenSpawn.Spawn(this.Props.offspring, cell, map);
                    count++;
                }
            }

            if (count >= adjCells.Count) active = false;
        }
        private void TryGrow()
        {
            if (this.Props.changeInto == null || this.parent == null || !this.active) return;

            if (Rand.Chance(this.Props.chance))
            {
                if (this.Props.changeInto.size.x > 1 || this.Props.changeInto.size.z > 1)
                {
                    Merge();
                }else
                {
                    GenSpawn.Spawn(this.Props.changeInto, this.parent.Position, this.parent.Map);
                }

            }

        }
        private void Merge()
        {
            /*
            if(this.parent == null || this.parent.Map == null) 
            {
                //Log.Message("parent or map is null");
                return;
            }*/

            IntVec3 location = this.parent.Position;
            Map map = this.parent.Map;
            IntVec2 thingSize = this.Props.changeInto.size;

            //Log.Message("Checking " + this.parent + " at " + location);
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
                    //Log.Message("Checking cell: " + checkCell.ToString());
                    found = checkCell.GetFirstThing(map, this.parent.def);
                    //Log.Message(this.parent.def + " found in cell");
                    if (!checkCell.InBounds(map) || found == null)
                    {
                        //Log.Message(this.parent.def + " not found in cell. terminating attempt");
                        mergeableThings.Clear();
                        break;
                    }   

                    //Log.Message(found.Label +  " | " + found.Position);
                    mergeableThings.Add(found);
                }

                if (mergeableThings.Count >= thingSize.Area)
                {
                    //Log.Message("Found things: " + mergeableThings.Count + "/" + thingSize.Area);
                    break;
                }
            }

            if (mergeableThings.Count >= thingSize.Area)
            {
                CellRect placementRect = new CellRect(newLocation.x, newLocation.z, thingSize.x, thingSize.z);
                if (!placementRect.InBounds(map)) return;

                //Log.Message("Placing thing at " + newLocation.ToString());
                GenSpawn.Spawn(this.Props.changeInto, newLocation, map);
            }
        }
        private void CheckActivity()
        {
            List<IntVec3> adjCells = GenAdjFast.AdjacentCells8Way(this.parent.PositionHeld).Where(c => {
                Building building = c.GetFirstBuilding(this.parent.MapHeld);
                if(building != null)
                {
                    return building.GetType().IsSubclassOf(typeof(Building_OriginiumCluster));
                }
                else { return false; }
            }).ToList();
            Log.Message(string.Join(",",adjCells));
            if(adjCells.Count < 8)
            {
                active = true;
            }
            else
            {
                active= false;
                this.parent.Destroy();
            }
            Log.Message("active: " + active);

        }
        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look<int>(ref this.cooldownTicksLeft, "cooldownTicksLeft", cooldownTicks);
            Scribe_Values.Look<bool>(ref this.ready, "ready", ready);
            Scribe_Values.Look<bool>(ref this.active, "active", active);
        }

        private int cooldownTicksLeft;

        private bool ready = false;

        private bool active = true;

    }
}
