using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using Verse;
using RimWorld;
using UnityEngine;

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
            this.TryTriggerCountdownShatter();
        }

        private void TryTriggerCountdownShatter()
        {
            //this.shattering = false;
            if (this.shattering)
            {
                Log.Message("Already shattering.");
                return;
            }
            Log.Message("Shattering countdown started for " + this.pawn.Name);
            this.shattering = true;
            this.timer = GenTicks.TicksGame + shatterCountdownSeconds.SecondsToTicks();
            //this.countdownTimer.Start(GenTicks.TicksGame, Hediff_Oripathy.shatterCountdownSeconds.SecondsToTicks(), new Action(this.Shatter));
        }

        public void TickRare()
        {
            if (!this.countdownTimer.Finished)
            {
                Log.Message("Timer not done. Current tick: " + GenTicks.TicksGame);
                this.countdownTimer.TickInterval();
            }
        }
        private void tryShatterCorpse()
        {
            if (GenTicks.TicksGame > timer)
            {
            GenExplosion.DoExplosion(this.pawn.Position, this.pawn.Map, 3f, DamageDefOf.Flame, this.pawn, 1, -1f, null, null, null, null, null, 0f, 1, null, false, null, 0f, 1, 0f, false, null, null, null, true, 1f, 0f, true, null, 1f, null, null);
            
                Log.Message("pawn is shattering");
                this.pawn.equipment.DestroyAllEquipment(DestroyMode.Vanish);
                this.pawn.apparel.DestroyAll(DestroyMode.Vanish);
            }
            else
            {
                Log.Message(pawn.Name + " still ticking.")
            }
            
        }

        private TickTimer countdownTimer = new TickTimer();

        public float timer;
        private static readonly float
            shatterCountdownSeconds = 0.5f;

        private bool shattering;

    }
}
