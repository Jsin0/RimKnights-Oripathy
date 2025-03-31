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
using Verse.Noise;

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
        
        public override void Notify_PawnDied(DamageInfo? dinfo, Hediff culprit = null)
        {
            base.Notify_PawnDied(dinfo, culprit);

            if (this.pawn.Faction == Faction.OfPlayer)
            {
                String name = this.pawn.Name.ToStringShort;
                Find.LetterStack.ReceiveLetter("RK_LetterLabelOripathicDeath".Translate(name), "RK_LetterOripathicDeath".Translate(name), LetterDefOf.NegativeEvent, new TargetInfo(this.pawn.Position, this.pawn.MapHeld, false), null, null, null, null, 0, true);
            }
            
            this.TryTriggerWarmupTimer();
            this.TryTriggerWarmupEffect();
        }
        private int GetFinalDelay()
        {
            return (int)((this.Severity * (-0.70) + 1 + Hediff_Oripathy.randDayDelay.RandomInRange) * 60000); //60000 converts from days to ticks
        }
        private void TryTriggerWarmupTimer()
        {
            if(this.pawn.Corpse != null)
            {
                this.shattering = true;
                this.warmupTimer.Start(GenTicks.TicksGame, this.GetFinalDelay(), new Action(this.TryShatter));
            }
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
                corpse.MapHeld.effecterMaintainer.AddEffecterToMaintain(this.shatterWarmupEffecter, corpse, 250);
            }
        }
        private void TryShatter()
        {
            if (this.pawn.Corpse == null)
            {
                Log.Error("RK_Oripathy: cannot shatter, corpse is null");
                return;
            }

            this.shatterTimer.Start(GenTicks.TicksGame, Hediff_Oripathy.shatterDurationSeconds.RandomInRange.SecondsToTicks(), new Action(this.DoShatterCorpse));
            this.TryTriggerShatterEffect();

            String name = this.pawn.Name.ToStringShort;
            Find.LetterStack.ReceiveLetter("RK_LetterLabelShattering".Translate(name), "RK_LetterShattering".Translate(name), LetterDefOf.NegativeEvent, new TargetInfo(this.pawn.Position, this.pawn.MapHeld, false), null, null, null, null, 0, true);
        }

        private void TryTriggerShatterEffect()
        {
            Corpse corpse;
            if ((corpse = this.pawn.ParentHolder as Corpse) != null && this.shattering && this.shatterEffecter == null)
            {
                this.shatterEffecter = EffecterDefOf.RK_Shattering.Spawn(corpse, corpse.MapHeld, Vector3.zero);
                corpse.MapHeld.effecterMaintainer.AddEffecterToMaintain(this.shatterEffecter, corpse, 250);
            }
        }
        public override void Tick()
        {
            base.Tick();
            Corpse corpse;
            if ((corpse = this.pawn.ParentHolder as Corpse) != null && this.shattering)
            {
                if (this.warmupTimer.Finished && this.shatterEffecter == null)
                {
                    this.shatterEffecter = EffecterDefOf.RK_Shattering.Spawn(corpse, corpse.MapHeld, Vector3.zero);
                    corpse.MapHeld.effecterMaintainer.AddEffecterToMaintain(this.shatterEffecter, corpse, 250);
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
                //Log.Message("Warmup timer not done.");
                this.warmupTimer.TickInterval();
                if(this.shatterWarmupEffecter != null)
                {
                    //Log.Message("prolonging warmup shatterEffecter");
                    this.shatterWarmupEffecter.ticksLeft += 250;
                }
                else
                {
                    TryTriggerWarmupEffect();
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
                if(this.shatterEffecter != null)
                {
                    this.shatterEffecter.ticksLeft = (this.shatterTimer.Finished ? 0 : (this.shatterEffecter.ticksLeft + 250));
                }
                else
                {
                    TryTriggerShatterEffect();
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
            if (shatterSustainer != null)
            {
                shatterSustainer.End();
            }
            if (shatterEffecter != null)
            {
                shatterEffecter.ForceEnd();
            }
            if (shatterWarmupEffecter != null)
            {
                shatterWarmupEffecter.ForceEnd();
            }
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
            }
        }


        private TickTimer warmupTimer = new TickTimer();

        private TickTimer shatterTimer = new TickTimer();

        private static readonly FloatRange
            shatterDurationSeconds = new FloatRange(45f, 75f);

        private bool shattering;

        private Effecter shatterWarmupEffecter;

        private Effecter shatterEffecter;

        private Sustainer shatterSustainer;

        private static readonly FloatRange
            randDayDelay = new FloatRange(-0.2f, 0.5f);

    }
}
