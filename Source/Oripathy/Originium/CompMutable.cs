using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
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

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            CheckActivity();
        }

        public override void PostPostApplyDamage(DamageInfo dinfo, float totalDamageDealt)
        {
            if (dinfo.Def == this.Props.trigger && !active) 
            {
                spreadCounter = 0;
                CheckActivity();
            }
        }
        
        public override void CompTick()
        {
            if (active)
            {
                if(parent.IsHashIntervalTick(250)) CompTickRare();
            }
        }
        public override void CompTickRare()
        {
            base.CompTickRare();
            if(active)
            {
                float spreadMTBHours = this.Props.spreadMTBHours;
                if (spreadMTBHours > 0)
                {
                    float spreadFactor = parent.Map.GameConditionManager.GetActiveCondition<GameCondition_OriginiumRain>()?.compMutableSpreadFactor ?? 1f;
                    //Log.Message("Spread Factor" + spreadFactor);
                    if (Rand.MTBEventOccurs(spreadMTBHours * spreadFactor, 2500f, 250f))
                    {
                        TrySpread();
                    }
                    if (Rand.MTBEventOccurs(spreadMTBHours * spreadFactor, 2500f, 250f))
                    {
                        TryGrow();
                    }
                }
            }
        }
        private void TrySpread()
        {
            if (this.Props.offspring != null && parent != null && canSpread)
            {
                Map map = parent?.MapHeld;
                if (map == null) return;

                List<IntVec3> adjCells = GenAdjFast.AdjacentCells8Way(parent).Where((IntVec3 x) => IsValidCell(x, map)).ToList();
                //Log.Message("adjCells = " + adjCells.Count);
                if(adjCells.Count == 0)
                {
                    //Log.Message("no valid cells adjacent");
                    canSpread = false;
                    return;
                }
                else
                {
                    Spread2(adjCells, map);
                }
            }
        }

        /*
        private void Spread(List<IntVec3> cells, Map map)
        {
            for (int i = 0; i < cells.Count; i++)
            {
                IntVec3 cell = cells[i];

                if (Rand.Chance(this.Props.chance))
                {

                    Building building = cell.GetFirstBuilding(map);
                    if (building != null)
                    {
                        
                        //Log.Message(building.GetType().ToString());
                        if (building.def == this.Props.offspring || building.GetType() == typeof(Building_OriginiumCluster))
                        {
                            DamageInfo dinfo = new DamageInfo(DamageDefOf.RK_ActiveOriginium, 1f);
                            building.TakeDamage(dinfo);
                            continue;
                        }

                        if (!cell.Walkable(map))
                        {
                            //Log.Message("damaging building");
                            DamageInfo dinfo = new DamageInfo(RimWorld.DamageDefOf.Stab, 100f);
                            building.TakeDamage(dinfo);
                            if (!building.Destroyed) continue;
                        }
                    }

                    GenSpawn.Spawn(this.Props.offspring, cell, map);
                    //canSpread = false;
                }
            }
        }*/

        private void Spread2(List<IntVec3> cells, Map map)
        {
            if (this.Props.offspring != null && parent != null && canSpread)
            {
                IntVec3 cell;
                if (cells.TryRandomElement(out cell))
                {
                    //Log.Message(cell.ToString());
                    Building building = cell.GetFirstBuilding(map);
                    if (building != null)
                    {
                        /*Log.Message(building.GetType().ToString());
                        if (building.GetType().IsAssignableFrom(parent.GetType()))
                        {
                            DamageInfo dinfo = new DamageInfo(DamageDefOf.RK_ActiveOriginium, 1f);
                            building.TakeDamage(dinfo);
                            return;
                        }*/

                        if (!cell.Walkable(map))
                        {
                            //Log.Message("damaging building");
                            DamageInfo dinfo = new DamageInfo(RimWorld.DamageDefOf.Stab, 100f);
                            building.TakeDamage(dinfo);
                            if (!building.Destroyed) return;
                        }
                    }
                    //Log.Message("spawning");
                    GenSpawn.Spawn(this.Props.offspring, cell, map);
                    if (spreadCounter++ >= Props.spreadCount)
                    {
                        active = false;
                        spreadCounter = 0;
                    }
                }
            }
        }
        private bool IsValidCell(IntVec3 cell, Map map)
        {
            if (cell == null || !cell.InBounds(map)) return false;
            Building building = cell.GetFirstBuilding(map);
            if (building == null)
            {   
                return cell.Walkable(map);
            }
            else return building.GetType() != parent.GetType() && !building.GetType().IsSubclassOf(parent.GetType());
        }
        private void TryGrow()
        {
            if(Props.changeInto == null) canGrow = false;

            if (!canGrow || !this.active || parent?.Map == null) return;

            {
                if (this.Props.changeInto.size.x > 1 || this.Props.changeInto.size.z > 1)
                {
                    Merge();
                }else
                {
                    GenSpawn.Spawn(this.Props.changeInto, parent.Position, parent.Map);
                }

            }

        }
        private void Merge()
        {
            
            Map map = parent?.MapHeld;

            if(map == null) 
            {
                //Log.Message("Merge:parent or map is null");
                return;
            }

            IntVec3 location = parent.PositionHeld;
            IntVec2 thingSize = this.Props.changeInto.size;

            //Log.Message("Checking " + parent + " at " + location);
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
                    if (!checkCell.InBounds(map))
                    {
                        //Log.Message(parent.def + " not found in cell. terminating attempt");
                        break;
                    }

                    found = checkCell.GetFirstThing(map, parent.def);
                    //Log.Message(parent.def + " found in cell");
                    if (found == null)
                    {
                        break;
                    }

                    mergeableThings.Add(found);
                }

                if (mergeableThings.Count >= thingSize.Area)
                {

                    CellRect placementRect = new CellRect(newLocation.x, newLocation.z, thingSize.x, thingSize.z);

                    if (!placementRect.InBounds(map)) return;

                    GenSpawn.Spawn(this.Props.changeInto, newLocation, map);

                    return;
                }
            }
        }
        private void CheckActivity()
        {
            if (parent?.MapHeld == null)
            {
                //Log.Message("map null");
                active = false;
                return;
            }

            this.canGrow = Props.changeInto != null && Props.spreadMTBHours > 0;
            this.canSpread = Props.offspring != null && Props.spreadMTBHours > 0;

            if (canSpread)
            {

                List<IntVec3> adjCells = GenAdjFast.AdjacentCells8Way(parent).Where((IntVec3 x) => IsValidCell(x, parent.MapHeld)).ToList();
                if (adjCells.Count == 0)
                {
                    //Log.Message("Can't spread");
                    canSpread = false;
                }
            }

            if (!canSpread && !canGrow) 
            { 
                active = false;
            }
            else
            {
                active = true;
            }
        }
        public override void PostExposeData()
        {
            base.PostExposeData();
            if(Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                CheckActivity();
            }
            Scribe_Values.Look<bool>(ref this.ready, "ready", false);
            Scribe_Values.Look<bool>(ref this.active, "active", false);
            Scribe_Values.Look<bool>(ref this.canSpread, "canSpread", false);
            Scribe_Values.Look<bool>(ref this.canGrow, "active", false);
            Scribe_Values.Look<int>(ref this.spreadCounter, "spreadCounter", 0);
        }

        private bool canSpread = true;

        private bool canGrow = true;

        private bool ready = false;

        private bool active = true;

        private int checkCounter = 0;

        private int spreadCounter = 0;

    }
}
