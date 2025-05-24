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
            this.cooldownTicksLeft = cooldownTicks;
            this.ready = false;
        }
        public override void PostPostApplyDamage(DamageInfo dinfo, float totalDamageDealt)
        {
            if (dinfo.Def == this.Props.trigger && ready) 
            {
                if (Rand.Chance(this.Props.chance))
                {
                    TryGrow();
                    TrySpread();
                }
            }
        }
        public override void CompTick()
        {
            if (!active) return;

            if (this.parent.IsHashIntervalTick(250))
            {
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
                    TrySpread();
                    TryGrow();
                    ResetCooldown();
                }
            }
        }
        public override void CompTickRare()
        {
            base.CompTickRare();
            Log.Message("tickrare");
        }
        public override void CompTickLong()
        {
            base.CompTickLong();
            Log.Message("long tick");
            CheckActivity();
        }
        private void TrySpread()
        {
            if (this.Props.offspring != null && this.parent != null)
            {
                //Log.Message("spreading at " + this.parent.Position);
                Spread();
                /*
                if (Rand.Chance(Props.chance))
                {
                    //Spread(this.parent.Position, this.parent.Map);
                    GenExplosion.DoExplosion(this.parent.Position, this.parent.Map, Props.effectRadius, DamageDefOf.RK_ActiveOriginium, this.parent, 25, -1, null, null, null, null, Props.offspring, Props.chance,1,null,false,null,0,1,0.1f,true,null,null,null,false,1,0,false,null,0,null,null);
                    ResetCooldown();
                }*/
            }
        }
        private void Spread()
        {
            List<IntVec3> adjCells = GenAdjFast.AdjacentCells8Way(this.parent);
            Map map = this.parent.Map;
            int count = 0;
            foreach (IntVec3 cell in adjCells)
            {
                if (!cell.InBounds(map) || cell == this.parent.Position) 
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
                        continue;
                    }
                }

                if (Rand.Chance(this.Props.chance))
                {
                    if (!cell.Walkable(map))
                    {
                        //Log.Message("damaging building");
                        DamageInfo dinfo = new DamageInfo(RimWorld.DamageDefOf.Stab, 25f);
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
            if (this.Props.changeInto == null || this.parent == null) return;

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
            IntVec3 location = this.parent.Position;
            Map map = this.parent.Map;
            IntVec2 thingSize = this.Props.changeInto.size;

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

                if (mergeableThings.Count >= thingSize.Area)
                {
                    break;
                }
            }

            if (mergeableThings.Count >= thingSize.Area)
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
                GenSpawn.Spawn(this.Props.changeInto, newLocation, map);
            }
        }
        private void CheckActivity()
        {
            List<IntVec3> adjCells = GenAdjFast.AdjacentCells8Way(this.parent.Position).Where(c => c.GetFirstBuilding(this.parent.Map).GetType().IsSubclassOf(typeof(Building_OriginiumCluster))).ToList();
            if(adjCells.Count < 8)
            {
                active = true;
            }
            else
            {
                active= false;
            }

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
