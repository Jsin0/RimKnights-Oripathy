using System;
using System.Collections.Generic;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;
using Verse.Noise;
using Verse.Sound;

namespace RimKnights.Oripathy
{
    public class Hediff_Oripathy : Hediff_OriginiumBase
    {
        private enum ShatterPhase
        {
            None,
            Warmup,
            Shatter,
            Complete
        }
        
        private bool notified = false;
        private ShatterPhase currentPhase = ShatterPhase.None;
        private TickTimer warmupTimer;
        private TickTimer shatterTimer;
        private static readonly FloatRange shatterDurationSeconds = new FloatRange(45f, 75f);
        private Effecter warmupEffecter;
        private Sustainer warmupSustainer;
        private Effecter shatterEffecter;
        private Sustainer shatterSustainer;
        private static readonly FloatRange randDayDelay = new FloatRange(-0.2f, 0.5f);
        private int finalDelay => (int)((this.Severity * (-0.70) + 1 + Hediff_Oripathy.randDayDelay.RandomInRange) * 60000); //60000 converts from days to ticks
        
        public override void Notify_PawnDied(DamageInfo? dinfo, Hediff culprit = null)
        {
            base.Notify_PawnDied(dinfo, culprit);
            Corpse corpse = pawn?.Corpse;
            if(!corpse.DestroyedOrNull()) TryTriggerWarmup();
            
            if (this.pawn.Faction == Faction.OfPlayer)
            {
                if (OripathyMod.infectionMonitor && !this.Visible) return;
                String name = this.pawn.Name.ToStringShort;
                Find.LetterStack.ReceiveLetter("RK_LetterLabelOripathicDeath".Translate(name), "RK_LetterOripathicDeath".Translate(name), LetterDefOf.NegativeEvent, corpse, null, null, null, null, 0, true);
            }
            if(corpse.MapHeld == null)
            {
                Caravan caravan = GetCaravanHoldingCorpse(corpse);
                if (caravan != null && OripathyMod.abandonOripathicCorpses) 
                {
                    RimWorld.Planet.CaravanAbandonOrBanishUtility.TryAbandonOrBanishViaInterface(corpse, caravan);
                }
            }
        }
        private void TryTriggerWarmup()
        {
            this.TryTriggerWarmupTimer();
            this.TryTriggerWarmupEffect();
        }
        private void TryTriggerShatter()
        {
            this.TryTriggerShatterTimer();
            this.TryTriggerShatterEffect();
        }
        private void TryTriggerWarmupTimer()
        {
            currentPhase = ShatterPhase.Warmup;
            if(this.warmupTimer == null) this.warmupTimer = new TickTimer();
            this.warmupTimer.Start(GenTicks.TicksGame, this.finalDelay, new Action(this.TryTriggerShatter));
            if (OripathyMod.settings.debugMode) Log.Message("DebugShatterWarmupStarted".Translate("pawn", pawn.NameFullColored));
        }
        private void TryTriggerWarmupEffect()
        {
            Corpse corpse = pawn.Corpse;
            if(corpse.MapHeld != null && !corpse.IsDessicated())
            {
                if (this.warmupEffecter == null)
                {
                    EffecterDef effecter = ModsConfig.BiotechActive ? RimWorld.EffecterDefOf.CellPollution : EffecterDefOf.RK_ShatterWarmup;
                    this.warmupEffecter = effecter.Spawn(corpse, corpse.MapHeld, Vector3.zero);
                    corpse.MapHeld.effecterMaintainer.AddEffecterToMaintain(this.warmupEffecter, corpse, 250);
                }
                if(this.warmupSustainer == null)
                {
                    SoundInfo soundInfo = SoundInfo.InMap(corpse, MaintenanceType.PerTickRare);
                    this.shatterSustainer = SoundDefOf.FireBurning.TrySpawnSustainer(soundInfo);
                }
            }
                
        }
        private void TryTriggerShatterTimer()
        {
            Corpse corpse = pawn?.Corpse;
            if (corpse.DestroyedOrNull())
            {
                if(OripathyMod.settings.debugMode) Log.Error("DebugAlreadyDestroyedCorpse".Translate());
                return;
            }

            currentPhase = ShatterPhase.Shatter;
            if (this.shatterTimer == null) this.shatterTimer = new TickTimer();
            this.shatterTimer.Start(GenTicks.TicksGame, Hediff_Oripathy.shatterDurationSeconds.RandomInRange.SecondsToTicks(), new Action(this.DoShatterCorpse));
            
            if (this.pawn.Faction == Faction.OfPlayer)
            {
                String name = this.pawn.Name.ToStringShort;
                Find.LetterStack.ReceiveLetter("RK_LetterLabelShattering".Translate(name), "RK_LetterShattering".Translate(name), LetterDefOf.NegativeEvent, corpse, null, null, null, null, 0, true);
            }

            Messages.Message("MessageShatteringCorpse".Translate(pawn.Named("PAWN")), corpse, MessageTypeDefOf.NegativeEvent);
            if (OripathyMod.settings.debugMode) Log.Message("DebugShatteringStart".Translate(pawn.Named("PAWN")));
        }

        private void TryTriggerShatterEffect()
        {
            Corpse corpse = pawn?.Corpse;
            if (!corpse.DestroyedOrNull() && corpse.MapHeld != null && this.shatterEffecter == null && !corpse.IsDessicated())
            {
                Thing glower = ThingMaker.MakeThing(ThingDefOf.RK_ShatterGlow);

                CompFollower compFollower;
                if(glower.TryGetComp(out compFollower))
                {
                    compFollower.SetTarget(corpse);
                    GenSpawn.Spawn(glower, corpse.Position, corpse.MapHeld);
                }

                this.shatterEffecter = EffecterDefOf.RK_Shattering.Spawn(corpse, corpse.MapHeld, Vector3.zero);
                corpse.MapHeld.effecterMaintainer.AddEffecterToMaintain(this.shatterEffecter, corpse, 250);
            }
        }

        public override void Tick()
        {
            base.Tick();
            if (!notified && Visible)
            {
                Find.HistoryEventsManager.RecordEvent(new HistoryEvent(HistoryEventDefOf.RK_BecameOripathic, pawn.Named(HistoryEventArgsNames.Doer)), true);
                notified = true;
            }
            if (pawn.IsHashIntervalTick(60000)) //once a day)
            {
                pawn.health.GetOrAddHediff(RimWorld.HediffDefOf.ToxicBuildup);
            }
        }
        public void TickRare()
        {
            Corpse corpse = pawn?.Corpse;
            if(currentPhase == ShatterPhase.Complete || corpse.DestroyedOrNull())
            {
                return;
            }
            switch (currentPhase)
            {
                case ShatterPhase.None:
                    TryTriggerWarmup(); 
                    break;
                case ShatterPhase.Warmup:
                    if (this.warmupTimer != null) this.warmupTimer.TickIntervalDelta();
                    else TryTriggerWarmup();

                    if (corpse.MapHeld != null && !corpse.IsDessicated())
                    {
                        if (this.warmupEffecter != null)
                        {
                            this.warmupEffecter.ticksLeft = this.warmupTimer.Finished ? 0 : this.warmupEffecter.ticksLeft + 250;
                        }
                        if (this.warmupSustainer != null && !this.warmupSustainer.Ended)
                        {
                            this.warmupSustainer.Maintain();
                        }
                        if (this.warmupEffecter == null || this.warmupSustainer == null)
                        {
                            TryTriggerWarmupEffect();
                        }
                    }
                    break;
                case ShatterPhase.Shatter:
                    if (this.shatterTimer != null) this.shatterTimer.TickIntervalDelta();
                    else TryTriggerShatter();

                    if(corpse.MapHeld != null && !corpse.IsDessicated())
                    {
                        if (this.shatterEffecter != null)
                        {
                            this.shatterEffecter.ticksLeft = this.shatterTimer.Finished ? 0 : this.shatterEffecter.ticksLeft + 250;
                        }
                        if (this.shatterSustainer != null && !this.shatterSustainer.Ended)
                        {
                            this.shatterSustainer.Maintain();
                        }
                        if (this.shatterEffecter == null || this.shatterSustainer == null)
                        {
                            TryTriggerShatterEffect();
                        }
                    }
                    break;
                case ShatterPhase.Complete:
                    break;

            }
        }
        private void DoShatterCorpse()
        {
            Corpse corpse = pawn?.Corpse; 
            if (!corpse.DestroyedOrNull())
            {
                if (corpse.MapHeld != null)
                {
                    this.shatterSustainer?.End();
                    IntVec3 center = corpse.PositionHeld;
                    Map map = corpse.MapHeld;

                    if (!corpse.IsDessicated() && TryDamageContainer(corpse))
                    {
                        float radius = Mathf.Max(this.pawn.BodySize, 0.5f) * 2f;
                        DamageDef damageDef = OripathyMod.originiumModActive ? damageDef = Originium.DamageDefOf.RK_ActiveOriginium : RimWorld.DamageDefOf.ToxGas;
                        ThingDef spawnedThingDef = null;
                        if (OripathyMod.originiumModActive) spawnedThingDef  = OriginiumInterOp.GetClusterDef();
                        GenExplosion.DoExplosion(center, map, radius, damageDef, corpse, -1, -1f, null, null, null, null, spawnedThingDef, 0.20f, 1, null, null,255, false, null, 0f, 1, 0.2f, true, null, null, null, true, 1f, 0f, true, null, 1f, null, null);
                    }

                    if (OripathyMod.originiumModActive) OriginiumInterOp.SpawnCluster(center, map);
                }
                else
                {
                    TryInfectCaravan(GetCaravanHoldingCorpse(corpse));
                }

                if (!corpse.DestroyedOrNull())
                {
                    Messages.Message("MessageShatteredCorpse".Translate(pawn.Named("PAWN")), MessageTypeDefOf.NegativeEvent);
                    corpse.Destroy(DestroyMode.Vanish);
                }
            }
        }
        private Caravan GetCaravanHoldingCorpse(Corpse corpse)
        {
            IThingHolder holder = corpse as IThingHolder;
            HashSet<IThingHolder> visited = new HashSet<IThingHolder>();

            while(holder != null)
            {
                if(holder is Caravan caravan) return caravan;

                if (!visited.Add(holder))
                {
                    //ThingHolder already checked. Stuck in a loop for some reason.
                    break;
                }

                holder = holder.ParentHolder;
            }
            return null;
        }
        private void TryInfectCaravan(Caravan caravan)
        {
            if (caravan != null)
            {
                if (OripathyMod.settings.debugMode) Log.Message("DebugShatterInCaravan".Translate("pawn", this.pawn.NameShortColored, "caravan", caravan.Name));

                FloatRange randSeverity = new FloatRange(0f, 0.4f);
                foreach (Pawn p in caravan.pawns)
                {
                    if (p != null)
                    {
                        Hediff buildupHediff = p.health.GetOrAddHediff(RimWorld.HediffDefOf.ToxicBuildup);
                        buildupHediff.Severity += randSeverity.RandomInRange;
                        if (OripathyMod.settings.debugMode) Log.Message("DebugCaravanBobSeverity".Translate("pawn", p.NameFullColored, "severity", buildupHediff.Severity));

                    }
                }
            }
        }
        private bool TryDamageContainer(Corpse corpse)
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
                    DamageDef damageDef = RimWorld.DamageDefOf.Bomb; //OripathyMod.originiumModActive ? Originium.DamageDefOf.OriginiumBlast : RimWorld.DamageDefOf.Bomb;
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
        public override void Notify_PawnCorpseDestroyed()
        {
            currentPhase = ShatterPhase.Complete;
            warmupTimer = null;
            shatterTimer = null;
            shatterSustainer?.End();
            shatterEffecter?.ForceEnd();
            warmupEffecter?.ForceEnd();
            warmupSustainer?.End();
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref currentPhase, "currentPhase", ShatterPhase.None);
            Scribe_Values.Look(ref notified, "notified", false);
            Scribe_Deep.Look<TickTimer>(ref this.warmupTimer, "warmupTimer", Array.Empty<object>());
            Scribe_Deep.Look<TickTimer>(ref this.shatterTimer, "shatterTimer", Array.Empty<object>());
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                if(this.warmupTimer != null) this.warmupTimer.OnFinish = new Action(this.TryTriggerShatter);
                if(this.shatterTimer != null) this.shatterTimer.OnFinish = new Action(this.DoShatterCorpse);
            }
        }

    }
}
