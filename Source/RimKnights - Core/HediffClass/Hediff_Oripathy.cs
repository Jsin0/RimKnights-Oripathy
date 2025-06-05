using RimWorld;
using System;
using UnityEngine;
using Verse;
using Verse.Noise;
using Verse.Sound;

namespace RimKnights
{
    public class Hediff_Oripathy : HediffVisibleWithApparel
    {
        public override void PostAdd(DamageInfo? dinfo)
        {
            base.PostAdd(dinfo);

            (this.pawn.health.GetOrAddHediff(HediffDefOf.RK_OriginiumBuildup) as Hediff_OriginiumBuildup).isOripathic = true;

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
            if (this.pawn.Corpse != null)
            {
                this.shattering = true;
                this.warmupTimer.Start(GenTicks.TicksGame, this.GetFinalDelay(), new Action(this.TryShatter));
            }
        }
        private void TryTriggerWarmupEffect()
        {
            if (!this.shattering)
            {
                return;
            }
            Corpse corpse;
            if((corpse = this.pawn.ParentHolder as Corpse) != null)
            {
                if (this.warmupEffecter == null)
                {
                    this.warmupEffecter = EffecterDefOf.RK_ShatterWarmup.Spawn(corpse, corpse.MapHeld, Vector3.zero);
                    corpse.MapHeld.effecterMaintainer.AddEffecterToMaintain(this.warmupEffecter, corpse, 250);
                }
                if(this.warmupSustainer == null)
                {
                    SoundInfo soundInfo = SoundInfo.InMap(corpse, MaintenanceType.PerTickRare);
                    this.shatterSustainer = SoundDefOf.FireBurning.TrySpawnSustainer(soundInfo);
                }
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
            if (pawn.IsHashIntervalTick(250))
            {
                TickRare();
            }
            /*
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
            }*/
        }
        public void TickRare()
        {
            Log.Message("shatter tickrare");
            if (!this.shattering)
            {
                return;
            }
            if (!this.warmupTimer.Finished)
            {
                //Log.Message("Warmup timer not done.");
                this.warmupTimer.TickInterval();
                if (this.warmupEffecter != null)
                {
                    //Log.Message("prolonging warmup shatterEffecter");
                    this.warmupEffecter.ticksLeft += 250;
                }
                if(this.warmupSustainer != null && !this.warmupSustainer.Ended)
                {
                    this.warmupSustainer.Maintain();

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
                if (this.shatterSustainer != null && !this.shatterSustainer.Ended)
                {
                    this.shatterSustainer.Maintain();
                }
                if (this.shatterEffecter != null)
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
            if (base.pawn != null)
            {
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
                    GenExplosion.DoExplosion(center, map, radius, RimKnights.DamageDefOf.RK_ActiveOriginium, base.pawn, -1, -1f, null, null, null, null, ThingDefOf.RK_OriginiumCluster, 0.20f, 1, null, false, null, 0f, 1, 0f, true, null, null, null, true, 1f, 0f, true, null, 1f, null, null);
                    GenSpawn.Spawn(RimKnights.ThingDefOf.RK_OriginiumCluster, center, map);

                }
                else
                {
                    Log.Error("corpse in null map, no explosion");
                }

                this.shattering = false;
                base.pawn.Corpse.Destroy(DestroyMode.Vanish);
            }
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
            if (warmupEffecter != null)
            {
                warmupEffecter.ForceEnd();
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

        private Effecter warmupEffecter;

        private Sustainer warmupSustainer;

        private Effecter shatterEffecter;

        private Sustainer shatterSustainer;

        private static readonly FloatRange
            randDayDelay = new FloatRange(-0.2f, 0.5f);

    }
}
