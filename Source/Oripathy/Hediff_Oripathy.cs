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

namespace Oripathy
{
    public class Hediff_Oripathy : HediffWithComps
    {
        
        /*public override void Notify_pawnKilled()
        {
            GenExplosion.DoExplosion(base.pawn.Position, base.pawn.Map, this.Props.explosionRadius, this.Props.damageDef, base.pawn, this.Props.damageAmount, -1f, null, null, null, null, null, 0f, 1, null, false, null, 0f, 1, 0f, false, null, null, null, true, 1f, 0f, true, null, 1f, null, null);
            if (this.Props.destroyGear)
            {
                base.pawn.equipment.DestroyAllEquipment(DestroyMode.Vanish);
                base.pawn.apparel.DestroyAll(DestroyMode.Vanish);
            }
        }*/

        public override void Notify_PawnDied(DamageInfo? dinfo, Hediff culprit = null)
        {
            base.Notify_PawnDied(dinfo, culprit);
            this.TryTriggerWarmupTimer();
        }
        private int GetFinalDelay()
        {
            int delay;
            delay = (int)((0.3 / this.Severity) * this.ticksDelay);
            //Log.Message(this.Severity);
            //Log.Message("delay = " + delay);
            if (delay > 180000) //3 days max
            {
                return 180000;
            }
            else
            {
                return delay;
            }
        }
        private void TryTriggerWarmupTimer()
        {
            if (this.shattering)
            {
                //Log.Message(this.pawn.Name + " is going to shatter soon.");
                return;
            }
            else
            {
                this.warmupTimer.Start(GenTicks.TicksGame, this.GetFinalDelay(), new Action(this.TryShatter));
                Log.Message("Oripathy: " + this.pawn.Name + " will soon shatter.");
            }

        }
        private void TryShatter()
        {
            //this.shattering = false;
            if (this.pawn.Corpse == null)
            {
                Log.Error("Oripathy: cannot shatter, corpse is null");
                return;
            }
            if (this.shattering)
            {
                Log.Error("Oripathy: " + this.pawn.Name + " is already shattering.");
                return;
            }
            Log.Message("Oripathy: Shattering started for " + this.pawn.Name);
            this.shattering = true;

            //this.timer = GenTicks.TicksGame + shatterDurationSeconds.SecondsToTicks();
            this.shatterTimer.Start(GenTicks.TicksGame, Hediff_Oripathy.shatterDurationSeconds.SecondsToTicks(), new Action(this.DoShatterCorpse));
            //TryTriggerShatterEffect();
        }

        private void TryTriggerShatterEffect()
        {
            
        }
        /*public override void Tick()
        {
            Corpse corpse;
            if ((corpse = this.pawn.ParentHolder as Corpse) != null)
            {
                if (this.effecter == null)
                {
                    this.effecter = EffecterDefOf.ExtinguisherExplosion.Spawn(corpse, this.pawn.MapHeld, Vector3.zero);
                    this.pawn.MapHeld.effecterMaintainer.AddEffecterToMaintain(this.effecter, corpse, 250);
                }
                if (this.shatterSustainer == null)
                {
                    SoundInfo soundInfo = SoundInfo.InMap(corpse, MaintenanceType.PerTickRare);
                    this.shatterSustainer = SoundDefOf.Tunnel.TrySpawnSustainer(soundInfo);
                }
            }
        
        }*/
        public void TickRare()
        {
            if (!this.warmupTimer.Finished)
            {
                //Log.Message("Warmup timer not done.");
                this.warmupTimer.TickInterval();
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
            else
            {
                //Log.Message("Shattering.");
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
                GenExplosion.DoExplosion(base.pawn.Position, base.pawn.MapHeld, 3f, DamageDefOf.OriginiumDust, base.pawn, 0, -1f, null, null, null, null, null, 0f, 1, null, false, null, 0f, 1, 0f, false, null, null, null, true, 1f, 0f, true, null, 1f, null, null);
            }
            else
            { 
                Log.Error("corpse in null map, no explosion");
            }
            
            //base.pawn.equipment.DestroyAllEquipment(DestroyMode.Vanish);
            //base.pawn.apparel.DestroyAll(DestroyMode.Vanish);
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
        

        private TickTimer warmupTimer = new TickTimer();

        private TickTimer shatterTimer = new TickTimer();

        private static readonly float
            shatterDurationSeconds = 10f;

        private bool shattering;

        private int ticksDelay = 60000;

        private Effecter effecter;

        private Sustainer shatterSustainer;



    }
}
