using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Oripathy
{
    public class HediffComp_ExplodeAfterDeath : HediffWithComps
    {
        public HediffComp_ExplodeAfterDeath Props
        {
            get
            { 
                return (HediffCompProperties_ExplodeAfterDeath)this.Props; 
            }
        }
        public override void Notify_PawnKilled()
        {
            GenExplosion.DoExplosion(base.Pawn.Position, base.Pawn.Map, this.Props.explosionRadius, this.Props.damageDef, base.Pawn, this.Props.damageAmount, -1f, null, null, null, null, null, 0f, 1, null, false, null, 0f, 1, 0f, false, null, null, null, true, 1f, 0f, true, null, 1f, null, null);
            if (this.Props.destroyGear)
            {
                base.Pawn.equipment.DestroyAllEquipment(DestroyMode.Vanish);
                base.Pawn.apparel.DestroyAll(DestroyMode.Vanish);
            }
        }

        public override void Notify_PawnDied(DamageInfo? dinfo, Hediff culprit = null)
        {
            if (this.Props.destroyBody)
            {
                base.Pawn.Corpse.Destroy(DestroyMode.Vanish);
            }
        }
    }
}
