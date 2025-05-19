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
                    Spread();
                    TryGrow();
                }
            }
        }
        public override void CompTick()
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
        private void Spread()
        {
            GenExplosion.DoExplosion(this.parent.Position, this.parent.Map, 1f, DamageDefOf.RK_ActiveOriginium, this.parent, -1, -1, null, null, null, null, Props.offspring, Props.chance);
            ResetCooldown();
        }
        private void TryGrow()
        {
            IntVec3 location = this.parent.Position;
            Map map = this.parent.Map;
            if (Props.changeInto.size.x > 1 || Props.changeInto.size.z >1)
            {
                IntVec3[] clusterOffsets = new IntVec3[]
                {
                    IntVec3.Zero,
                    IntVec3.North,
                    IntVec3.East,
                    IntVec3.NorthEast
                };

                List<Thing> toRemove = new List<Thing>();

                foreach(IntVec3 offset in clusterOffsets)
                {
                    IntVec3 checkCell = this.parent.Position + offset;
                    if (!checkCell.InBounds(map)) return;

                    Thing found = checkCell.GetFirstThing(map, this.parent.def);
                    if (found == null) return;

                    toRemove.Add(found);
                }

                CellRect placementRect = new CellRect(location.x, location.z, 2, 2);
                if (! placementRect.InBounds(map)) return;  

                foreach (IntVec3 placementCell in placementRect)
                {
                    if(!placementCell.Standable(map)) return;
                }

                foreach (Thing t in toRemove)
                {
                    t.Destroy(DestroyMode.Vanish);
                }

            }
                Thing newBuilding = ThingMaker.MakeThing(Props.changeInto);
                GenSpawn.Spawn(newBuilding, location, map, Rot4.North);
        }
        private void Merge()
        {

        }
        private void Grow()
        {
            IntVec3 location = this.parent.Position;
            Map map = this.parent.Map;
            this.parent.Destroy();
            GenSpawn.Spawn(Props.changeInto, location, map);
        }

        private int cooldownTicksLeft;

        private bool ready = false;

    }
}
