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
            //this.timer = GenTicks.TicksGame + shatterCountdownSeconds.SecondsToTicks();
            this.countdownTimer.Start(GenTicks.TicksGame, Hediff_Oripathy.shatterCountdownSeconds.SecondsToTicks(), new Action(this.tryShatterCorpse));
        }

        public void TickRare()
        {
            if (!this.countdownTimer.Finished)
            {
                Log.Message("Timer not done. Current tick: " + GenTicks.TicksGame);
                this.countdownTimer.TickInterval();
            }
            else
            {
                Log.Message("Timer done. Shattering");
            }
        }
        private void tryShatterCorpse()
        {
            if (base.pawn.MapHeld != null)
            {
                Log.Message(base.pawn.Name + " is shattering.");
                GenExplosion.DoExplosion(base.pawn.Position, base.pawn.MapHeld, 3f, DamageDefOf.Flame, base.pawn, 0, -1f, null, null, null, null, null, 0f, 1, null, false, null, 0f, 1, 0f, false, null, null, null, true, 1f, 0f, true, null, 1f, null, null);
            }
            else
            { 
                Log.Message("corpse in null map, no explosion");
            }
            
            base.pawn.equipment.DestroyAllEquipment(DestroyMode.Vanish);
            base.pawn.apparel.DestroyAll(DestroyMode.Vanish);
            base.pawn.Corpse.Destroy(DestroyMode.Vanish);
        }

        private TickTimer countdownTimer = new TickTimer();

        private static readonly float
            shatterCountdownSeconds = 10f;

        private bool shattering;

    }
}
