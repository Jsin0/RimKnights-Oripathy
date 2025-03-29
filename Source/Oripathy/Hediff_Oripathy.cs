using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using Verse;
using RimWorld;
using UnityEngine;
using Verse.Sound;

namespace Originium
{
    public class Hediff_Oripathy : HediffWithComps
    {

        public override void PostAdd(DamageInfo? dinfo)
        {
            if (!this.pawn.RaceProps.IsFlesh)
            {
                Log.Error("Tried giving oripathy to an inorganic pawn");
                this.pawn.health.RemoveHediff(this);
                return;
            }
            base.PostAdd(dinfo);

        }
        /*
        public override void PostRemoved()
        {
            base.PostRemoved();
            this.pawn.story.traits.RemoveTrait(new Trait(Originium.TraitDefOf.Oripathic));
        }*/
        
        public override void Notify_PawnDied(DamageInfo? dinfo, Hediff culprit = null)
        {
            base.Notify_PawnDied(dinfo, culprit);
            this.TryTriggerWarmupTimer();
            this.TryTriggerWarmupEffect();
        }
        private int GetFinalDelay()
        {
            return (int)((this.Severity * (-2.5) + 3 + Hediff_Oripathy.randDayDelay.RandomInRange) * 60000); //converts from days to ticks
        }
        private void TryTriggerWarmupTimer()
        {
            this.shattering = true;
            this.warmupTimer.Start(GenTicks.TicksGame, this.GetFinalDelay(), new Action(this.TryShatter));
            //Log.Message("Originium: " + this.pawn.Name + " will soon shatter soon");
        }
        private void TryTriggerWarmupEffect()
        {
            if(!this.shattering)
            {
                return;
            }
            Corpse corpse;
            if (this.shatterWarmupEffecter == null && (corpse = this.pawn.ParentHolder as Corpse) != null)
            {
                this.shatterWarmupEffecter = EffecterDefOf.RK_ShatterWarmup.Spawn(corpse, corpse.MapHeld, Vector3.zero);
                this.pawn.MapHeld.effecterMaintainer.AddEffecterToMaintain(this.shatterWarmupEffecter, corpse, 250);
            }
        }
        private void TryShatter()
        {
            if (this.pawn.Corpse == null)
            {
                Log.Error("RK_Oripathy: cannot shatter, corpse is null");
                return;
            }

            /*Messages.Message(this.pawn.Name + "'s corpse will soon shatter.", MessageTypeDefOf.NegativeEvent);*/
            this.shatterTimer.Start(GenTicks.TicksGame, Hediff_Oripathy.shatterDurationSeconds.RandomInRange.SecondsToTicks(), new Action(this.DoShatterCorpse));
            this.TryTriggerShatterEffect();
        }

        private void TryTriggerShatterEffect()
        {
            Corpse corpse;
            if ((corpse = this.pawn.ParentHolder as Corpse) != null && this.shattering && this.effecter == null)
            {
                Log.Message("shatter effect");
                this.effecter = EffecterDefOf.RK_ShatterWarmup.Spawn(corpse, corpse.MapHeld, Vector3.zero);
                this.pawn.MapHeld.effecterMaintainer.AddEffecterToMaintain(this.effecter, corpse, 250);
            }
        }
        public override void Tick()
        {
            Corpse corpse;
            if ((corpse = this.pawn.ParentHolder as Corpse) != null && this.shattering)
            {
                if (this.warmupTimer.Finished && this.effecter == null)
                {
                    Log.Message("cell pollustion spawned");
                    this.effecter = EffecterDefOf.RK_Shattering.Spawn(corpse, corpse.MapHeld, Vector3.zero);
                    corpse.MapHeld.effecterMaintainer.AddEffecterToMaintain(this.effecter, corpse, 250);
                }
                if (this.shatterSustainer == null)
                {
                    Log.Message("Sound sustainer");
                    SoundInfo soundInfo = SoundInfo.InMap(corpse, MaintenanceType.PerTickRare);
                    this.shatterSustainer = SoundDefOf.Tunnel.TrySpawnSustainer(soundInfo);
                }
            }
        }
        public void TickRare()
        {
            if (!this.shattering)
            {
                return;
            }
            if (!this.warmupTimer.Finished)
            {
                Log.Message("Warmup timer not done.");
                this.warmupTimer.TickInterval();
                if(this.shatterWarmupEffecter != null)
                {
                    Log.Message("prolonging warmup effecter");
                    this.shatterWarmupEffecter.ticksLeft += 250;
                }
            }
            else if (!this.shatterTimer.Finished)
            {
                //Log.Message("Shatter timer not done.");
                this.shatterTimer.TickInterval();
                if(this.shatterSustainer != null && !this.shatterSustainer.Ended)
                {
                    Sustainer sustainer = this.shatterSustainer;
                    if (sustainer != null)
                    {
                        sustainer.Maintain();
                    }
                }
                if(this.effecter != null)
                {
                    this.effecter.ticksLeft = (this.shatterTimer.Finished ? 0 : (this.effecter.ticksLeft + 250));
                }
            }
        }
        private void DoShatterCorpse()
        {
            //Log.Message("countdown timer done.");
            if (base.pawn.MapHeld != null)
            {
                //Log.Message(base.pawn.Name + " is shattering.");
                Sustainer sustainer = this.shatterSustainer;
                if (sustainer != null)
                {
                    sustainer.End();
                }
                IntVec3 center = base.pawn.Position;
                Map map = base.pawn.MapHeld;
                float radius = base.pawn.BodySize * 2f;
                GenExplosion.DoExplosion(center, map, radius, Originium.DamageDefOf.RK_OriginiumDust, base.pawn, 1, -1f, null, null, null, null, ThingDefOf.RK_OriginiumCluster, 0.20f, 1, null, false, null, 0f, 1, 0f, false, null, null, null, true, 1f, 0f, true, null, 1f, null, null);
                GenSpawn.Spawn(Originium.ThingDefOf.RK_OriginiumCluster, center, map);
                if (ModsConfig.BiotechActive) {PollutionUtility.GrowPollutionAt(center, map, (int)Math.Round(3.15 * radius * radius));}
               
            }
            else
            { 
                Log.Error("corpse in null map, no explosion");
            }

            this.shattering = false;
            base.pawn.Corpse.Destroy(DestroyMode.Vanish);
        }

        public override void Notify_PawnCorpseDestroyed()
        {
            Sustainer sustainer = this.shatterSustainer;
            if (sustainer == null)
            {
                return;
            }
            sustainer.End();
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Deep.Look<TickTimer>(ref this.warmupTimer, "warmupTimer", Array.Empty<object>());
            Scribe_Deep.Look<TickTimer>(ref this.shatterTimer, "shatterTimer", Array.Empty<object>());
            Scribe_Values.Look<bool>(ref this.shattering, "shattering", false);
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                this.shatterTimer.OnFinish = new Action(this.DoShatterCorpse);
                this.warmupTimer.OnFinish = new Action(this.TryShatter);
                if (!this.shattering && this.pawn.Dead)
                {
                    TryTriggerWarmupTimer();
                }
            }
        }

        private TickTimer warmupTimer = new TickTimer();

        private TickTimer shatterTimer = new TickTimer();

        private static readonly FloatRange
            shatterDurationSeconds = new FloatRange(30f, 60f);

        private bool shattering;

        private int ticksDelay = 60000;

        private Effecter shatterWarmupEffecter;

        private Effecter effecter;

        private Sustainer shatterSustainer;

        private static readonly FloatRange
            randDayDelay = new FloatRange(-0.1f, 0.7f);

    }
}
