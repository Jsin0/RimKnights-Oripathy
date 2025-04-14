using System;
using RimWorld;
using Verse;
using SCGF;

namespace Originium.SGC.Actions
{
    public class DoToxicDamage : GasAction
    {
        public override void DoEffects(Pawn pawn, byte gasDensity)
        {
            float num = (float)gasDensity / 255f;
            Pawn_HealthTracker health = pawn.health;
            Hediff hediff;
            if (health == null)
            {
                hediff = null;
            }
            else
            {
                HediffSet hediffSet = health.hediffSet;
                hediff = ((hediffSet != null) ? hediffSet.GetFirstHediffOfDef(Originium.HediffDefOf.RK_OriginiumBuildup, false) : null);
            }
            Hediff hediff2 = hediff;
            bool flag = hediff2 != null && hediff2.CurStageIndex == hediff2.def.stages.Count;
            if (flag)
            {
                num *= 0.25f;
            }
            GameCondition_OriginiumRain.DoPawnToxicDamage(pawn, false, false, num);
        }
    }
}
