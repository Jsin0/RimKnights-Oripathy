using System;
using RimWorld;
using Verse;

namespace Originium
{
    public class DamageWorker_ApplyHediff : DamageWorker
    {
        public override DamageWorker.DamageResult Apply(DamageInfo dinfo, Thing victim)
        {
            DamageWorker.DamageResult damageResult = new DamageWorker.DamageResult();
            Pawn pawn = victim as Pawn;
            if (pawn != null) 
            {
                Hediff hediff = HediffMaker.MakeHediff(dinfo.Def.hediff, pawn, null);
                hediff.Severity = dinfo.Amount/100;
                pawn.health.AddHediff(hediff, null, new DamageInfo?(dinfo), null);
            }
            return damageResult;
        }

        public FloatRange severityRange = FloatRange.Zero;
    }
}
