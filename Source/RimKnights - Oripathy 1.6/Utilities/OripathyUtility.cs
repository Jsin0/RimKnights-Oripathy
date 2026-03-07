using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using RimWorld.Planet;
using Unity.Mathematics;
using UnityEngine;
using Verse;

namespace RimKnights.Oripathy
{
    public static class OripathyUtility
    {
        private static DamageDef cachedShatterDef;
        public static DamageDef ShatterExplosionDef
        {
            get
            {
                if(cachedShatterDef == null)
                {
                    cachedShatterDef = DefDatabase<DamageDef>.GetNamed("RK_ShatterExplosion");
                }
                return cachedShatterDef;
            }
        }
        public static void SpreadOriginiumBuildup(IntVec3 center, Map map, float radius, float baseSeverity)
        {
            foreach (Pawn p in GenRadial.RadialDistinctThingsAround(center, map, radius, true).OfType<Pawn>().Where(p => p.RaceProps.IsFlesh))
            {
                float distanceFactor = 1 - (p.Position.DistanceTo(center) / radius);
                DoPawnOriginiumDamage(p, baseSeverity, distanceFactor);
            }
        }
        public static void DoPawnOriginiumDamage(Pawn pawn, float severity = 0.6f, float extraFactor = 1f)
        {
            //Based off DoPawnToxicDamage
            severity *= Mathf.Max(1f - pawn.GetStatValue(StatDefOf.RK_OriginiumResistance, true, -1), 0f);
            severity *= Mathf.Max(1f - pawn.GetStatValue(RimWorld.StatDefOf.ToxicEnvironmentResistance, true, -1), 0f);
            severity *= extraFactor;
            if (severity != 0f)
            {
                float variation = Mathf.Lerp(0.85f, 1.15f, Rand.ValueSeeded(pawn.thingIDNumber ^ 74374237));
                severity *= variation;
                HealthUtility.AdjustSeverity(pawn, HediffDefOf.RK_OriginiumBuildup, severity);
            }
        }
        public static Caravan GetCaravanHoldingCorpse(Corpse corpse)
        {
            IThingHolder holder = corpse as IThingHolder;
            HashSet<IThingHolder> visited = new HashSet<IThingHolder>();

            while (holder != null)
            {
                if (holder is Caravan caravan) return caravan;

                if (!visited.Add(holder))
                {
                    //ThingHolder already checked. Stuck in a loop for some reason.
                    break;
                }

                holder = holder.ParentHolder;
            }
            return null;
        }
        public static void TryInfectCaravan(Pawn pawn, Caravan caravan)
        {
            if (caravan != null)
            {
                if (OripathyMod.settings.debugMode) Log.Message("DebugShatterInCaravan".Translate(pawn.LabelShort.Named("pawn"), caravan.Name.Named("caravan")));

                FloatRange randSeverity = new FloatRange(0f, 0.6f);
                foreach (Pawn p in caravan.pawns)
                {
                    if (p != null)
                    {
                        DoPawnOriginiumDamage(p, randSeverity.RandomInRange);
                        Hediff buildupHediff = p.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.RK_OriginiumBuildup);
                        if (OripathyMod.settings.debugMode && buildupHediff != null) Log.Message("DebugCaravanBobSeverity".Translate(p.LabelShort.Named("pawn"), buildupHediff.Severity.Named("severity")));

                    }
                }
            }
        }
        public static bool TryDamageContainer(Corpse corpse)
        {
            IThingHolder container = corpse.ParentHolder;
            if (container is Pawn pawn)
            {
                if (pawn.carryTracker?.CarriedThing == corpse)
                {
                    pawn.carryTracker.TryDropCarriedThing(pawn.PositionHeld, ThingPlaceMode.Near, out Thing droppedCorpse);
                    droppedCorpse.SetForbidden(true);
                    return true;
                }
                if (pawn.inventory?.innerContainer.Contains(corpse) == true)
                {
                    pawn.inventory.innerContainer.TryDrop(corpse, ThingPlaceMode.Near, out Thing droppedCorpse);
                    droppedCorpse.SetForbidden(true);
                    return true;
                }
                return false;
            }
            else if (container is Building building)
            {
                if (building.def.useHitPoints)
                {
                    DamageDef damageDef = DamageDefOf.RK_OriginiumBlast;
                    DamageInfo damageInfo = new DamageInfo(damageDef, 200f);

                    building.TakeDamage(damageInfo);
                    if (building.Destroyed)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (building is Building_Casket casket)
                {
                    casket.EjectContents();
                    return true;
                }
            }
            return true;
        }
    }
}
