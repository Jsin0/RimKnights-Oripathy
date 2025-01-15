using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using Verse;
using RimWorld;
using UnityEngine;
using Verse.AI;
using Verse.Sound;

namespace Oripathy
{
    public class HediffComp_ExplodeAfterDeath : HediffComp
    {
        public HediffCompProperties_ExplodeAfterDeath Props
        {
            get
            { 
                return (HediffCompProperties_ExplodeAfterDeath)
                this.Props; 
            }
        }
        /*public override void Notify_PawnKilled()
        {
            GenExplosion.DoExplosion(base.Pawn.Position, base.Pawn.Map, this.Props.explosionRadius, this.Props.damageDef, base.Pawn, this.Props.damageAmount, -1f, null, null, null, null, null, 0f, 1, null, false, null, 0f, 1, 0f, false, null, null, null, true, 1f, 0f, true, null, 1f, null, null);
            if (this.Props.destroyGear)
            {
                base.Pawn.equipment.DestroyAllEquipment(DestroyMode.Vanish);
                base.Pawn.apparel.DestroyAll(DestroyMode.Vanish);
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
            Log.Message("Shattering countdown started for " + this.Pawn.Name);
            this.shattering = true;
            this.countdownTimer.Start(GenTicks.TicksGame, HediffComp_ExplodeAfterDeath.shatterCountdownSeconds.SecondsToTicks(), new Action(this.Shatter));
        }

        public void TickRare()
        { 
            if (!this.countdownTimer.Finished)
            {
                Log.Message("Timer not done. Current tick: " + GenTicks.TicksGame);
                this.countdownTimer.TickInterval();
            }
        }
        private void Shatter()
        {
            GenExplosion.DoExplosion(base.Pawn.Position, base.Pawn.Map, this.Props.explosionRadius, this.Props.damageDef, base.Pawn, this.Props.damageAmount, -1f, null, null, null, null, null, 0f, 1, null, false, null, 0f, 1, 0f, false, null, null, null, true, 1f, 0f, true, null, 1f, null, null);
            if (this.Props.destroyGear)
            {
                Log.Message("pawn is shattering");
                base.Pawn.equipment.DestroyAllEquipment(DestroyMode.Vanish);
                base.Pawn.apparel.DestroyAll(DestroyMode.Vanish);
            }
        }

        private TickTimer countdownTimer = new TickTimer();

        private static readonly float
            shatterCountdownSeconds = 0.5f;

        private bool shattering;
    }
}
