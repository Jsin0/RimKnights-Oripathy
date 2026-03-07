using System;
using System.Collections.Generic;
using System.Threading;
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
            if (corpse.DestroyedOrNull()) return;
                
            TryTriggerWarmup();
            
            if (this.pawn.Faction == Faction.OfPlayer)
            {
                if (OripathyMod.settings.infectionMonitor && !Visible) return;
                string name = this.pawn.LabelShort;
                Find.LetterStack.ReceiveLetter("RK_LetterLabelOripathicDeath".Translate(name), "RK_LetterOripathicDeath".Translate(name), LetterDefOf.NegativeEvent, corpse, null, null, null, null, 0, true);
            }
            if(corpse.MapHeld == null)
            {
                Caravan caravan = OripathyUtility.GetCaravanHoldingCorpse(corpse);
                if (caravan != null && Visible && OripathyMod.settings.abandonOripathicCorpses) 
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
            if (OripathyMod.settings.debugMode) Log.Message("DebugShatterWarmupStarted".Translate(pawn.LabelShort.Named("pawn")));
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
                    this.warmupSustainer = SoundDefOf.Tunnel.TrySpawnSustainer(soundInfo);
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
                string name = this.pawn.LabelShort;
                Find.LetterStack.ReceiveLetter("RK_LetterLabelShattering".Translate(name), "RK_LetterShattering".Translate(name), LetterDefOf.NegativeEvent, corpse, null, null, null, null, 0, true);
            }

            Messages.Message("MessageShatteringCorpse".Translate(pawn.Named("PAWN")), corpse, MessageTypeDefOf.NegativeEvent);
            if (OripathyMod.settings.debugMode) Log.Message("DebugShatteringStart".Translate(pawn.Named("PAWN")));
            corpse.SetForbidden(true);
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

                if (shatterEffecter == null)
                {
                    shatterEffecter = EffecterDefOf.RK_Shattering.Spawn(corpse, corpse.MapHeld, Vector3.zero);
                    corpse.MapHeld.effecterMaintainer.AddEffecterToMaintain(this.shatterEffecter, corpse, 250);
                }
                if (shatterSustainer == null)
                {
                    SoundInfo soundInfo = SoundInfo.InMap(corpse, MaintenanceType.PerTickRare);
                    shatterSustainer = SoundDefOf.FireBurning.TrySpawnSustainer(soundInfo);

                }
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
                pawn.health.GetOrAddHediff(HediffDefOf.RK_OriginiumBuildup);
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

                    if (!corpse.IsDessicated() && OripathyUtility.TryDamageContainer(corpse))
                    {
                        float radius = Mathf.Max(this.pawn.BodySize, 0.5f) * 2f;

                        GasType gasType = GasType.BlindSmoke;
                        ThingDef spawnedThingDef = null;
                        float spawnThingChance = 0;
                        
                        if (OripathyMod.originiumModActive) {
                            spawnedThingDef = OriginiumInterOp.GetClusterDef();
                            spawnThingChance = 0.2f;
                        }
                        if (ModLister.BiotechInstalled) {
                            gasType = GasType.ToxGas;
                        }

                        GenExplosion.DoExplosion(
                            center: center,
                            map: map,
                            radius: radius,
                            damType: DamageDefOf.RK_ActiveOriginium,
                            instigator: corpse,
                            damAmount: -1,
                            armorPenetration: -1f,
                            explosionSound: null,
                            weapon: null,
                            projectile: null,
                            intendedTarget: null,
                            postExplosionSpawnThingDef: spawnedThingDef,
                            postExplosionSpawnChance: spawnThingChance,
                            postExplosionSpawnThingCount: 0,
                            postExplosionGasType: gasType,
                            postExplosionGasRadiusOverride: null,
                            postExplosionGasAmount: 100,
                            applyDamageToExplosionCellsNeighbors: false,
                            preExplosionSpawnThingDef: null,
                            preExplosionSpawnChance: 0,
                            preExplosionSpawnThingCount: 1,
                            chanceToStartFire: 0.05f,
                            damageFalloff: true,
                            direction: null,
                            ignoredThings: null,
                            affectedAngle: null,
                            doVisualEffects: true,
                            propagationSpeed: 1f,
                            excludeRadius: 0,
                            doSoundEffects: true,
                            postExplosionSpawnThingDefWater: null,
                            screenShakeFactor: 1f,
                            flammabilityChanceCurve: null,
                            overrideCells: null,
                            postExplosionSpawnSingleThingDef: null,
                            preExplosionSpawnSingleThingDef: null
                            );

                    }

                    if (OripathyMod.originiumModActive) OriginiumInterOp.SpawnCluster(center, map);
                }
                else
                {
                    OripathyUtility.TryInfectCaravan(pawn, OripathyUtility.GetCaravanHoldingCorpse(corpse));
                }

                if (!corpse.DestroyedOrNull())
                {
                    Messages.Message("MessageShatteredCorpse".Translate(pawn.Named("PAWN")), MessageTypeDefOf.NegativeEvent);
                    corpse.Destroy(DestroyMode.Vanish);
                }
            }
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
