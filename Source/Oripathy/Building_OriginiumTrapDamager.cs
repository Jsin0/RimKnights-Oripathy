using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using Verse.Sound;

namespace Originium
{
    public class Building_OriginiumTrapDamager : Building_Trap
    {
        protected override void SpringSub(Pawn p)
        {
            if (base.Spawned)
            {
                SoundDefOf.TrapSpring.PlayOneShot(new TargetInfo(base.Position, base.Map, false));
            }
            if (p == null)
            {
                return;
            }
            float num = this.GetStatValue(StatDefOf.TrapMeleeDamage, true, -1) * Building_OriginiumTrapDamager.DamageRandomFactorRange.RandomInRange;
            float num2 = num * 0.015f;
            int num3 = 0;
            while ((float)num3 < 5f)
            {
                DamageInfo damageInfo = new DamageInfo(Originium.DamageDefOf.RK_OriginiumCut, num, num2, -1f, this, null, null, DamageInfo.SourceCategory.ThingOrUnknown, null, true, true, QualityCategory.Normal, true);
                DamageWorker.DamageResult damageResult = p.TakeDamage(damageInfo);
                if (num3 == 0)
                {
                    BattleLogEntry_DamageTaken battleLogEntry_DamageTaken = new BattleLogEntry_DamageTaken(p, RulePackDefOf.DamageEvent_TrapSpike, null);
                    Find.BattleLog.Add(battleLogEntry_DamageTaken);
                    damageResult.AssociateWithLog(battleLogEntry_DamageTaken);
                }
                num3++;
            }
        }
        private static readonly FloatRange DamageRandomFactorRange = new FloatRange(0.8f, 1.2f);
    }
}
