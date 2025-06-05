using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using UnityEngine;
using Verse;

namespace RimKnights
{
    public class CompHazardTouch : ThingComp
    {
        //Essentially just the Building_TrapDamager class as a thingComp
        public CompProperties_HazardTouch Props
        {
            get
            {
                return (CompProperties_HazardTouch)this.props;
            }
        }

        public override void CompTick()
        {
            //Log.Message("RareTick");
            if (parent.Spawned && parent.IsHashIntervalTick(10))
            {
                List<Thing> thingList = parent.Position.GetThingList(parent.Map);

                for (int i = 0; i < thingList.Count; i++)
                {
                    Pawn pawn = thingList[i] as Pawn;
                    if(pawn != null && !this.touchingPawns.Contains(pawn))
                    {
                        this.touchingPawns.Add(pawn);
                        this.CheckSpring(pawn);
                    }
                }
                for(int j = 0;j < touchingPawns.Count; j++)
                {
                    Pawn pawn2 = this.touchingPawns[j];
                    if(pawn2 == null || !pawn2.Spawned || pawn2.Position != parent.Position)
                    {
                        this.touchingPawns.Remove(pawn2);
                    }
                }
            }
            base.CompTick();
        }

        private void CheckSpring(Pawn pawn)
        {
            if (Rand.Chance(this.SpringChance(pawn)))
            {
                Log.Message("damaging pawn");
                Map map = parent.Map;
                this.Spring(pawn);
            }
        }

        private void Spring(Pawn pawn)
        {
            float damage = Props.baseDamage * DamageRandomFactorRange.RandomInRange / Props.numOfAttacks;
            float armorPen = damage * 0.015f;
            int numAttacks = 0;
            while ((float) numAttacks < Props.numOfAttacks)
            {
                Log.Message("damage: " + damage);
                DamageInfo damageInfo = new DamageInfo(Props.damageDef, damage, armorPen, -1, parent, null, null, DamageInfo.SourceCategory.ThingOrUnknown, null, true, false, QualityCategory.Normal, true);
                DamageWorker.DamageResult damageResult = pawn.TakeDamage(damageInfo);

                numAttacks++;
            }
        }

        private float SpringChance(Pawn pawn)
        {
            float num = 1f;
            if (pawn.kindDef.immuneToTraps)
            {
                return 0f;
            }
            if (pawn.IsNonMutantAnimal)
            {
                num = 0.2f;
                num *= Props.trapPeacefulWildAnimalsSpringChanceFactor;
            }else
            {
                num = 0.3f;
            }
            num *= Props.baseSpringChance * pawn.GetStatValue(RimWorld.StatDefOf.PawnTrapSpringChance, true, -1);
            return Mathf.Clamp01(num);
        }

        List<Pawn> touchingPawns = new List<Pawn>();

        private static readonly FloatRange DamageRandomFactorRange = new FloatRange(0.8f, 1.2f);

    }
}
