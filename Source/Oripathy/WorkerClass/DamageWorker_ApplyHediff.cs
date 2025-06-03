using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using UnityEngine;

namespace Originium
{
    public class DamageWorker_ApplyHediff : DamageWorker
    {
        public override DamageWorker.DamageResult Apply(DamageInfo dinfo, Thing victim)
        {
            DamageWorker.DamageResult damageResult = new DamageWorker.DamageResult();
            float damage = dinfo.Amount;
            Pawn pawn = victim as Pawn;
            HediffDef hediffDef = dinfo.Def.hediff;
            if (pawn != null) 
            {
                if(hediffDef != null)
                {
                    Hediff hediff = HediffMaker.MakeHediff(hediffDef, pawn, null);
                    hediff.Severity = 1f;
                    pawn.health.AddHediff(hediff, null, new DamageInfo?(dinfo), null);
                }
                //harmsHealth = false prevents additional hediffs from applying since no damage is done to pawns
                //basically copied from the Pawn_HealthTracker.PostApplyDamage method
                if (dinfo.Def.additionalHediffs != null && !dinfo.Def.harmsHealth)
                {
                    List<DamageDefAdditionalHediff> additionalHediffs = dinfo.Def.additionalHediffs;
                    for (int i = 0; i < additionalHediffs.Count; ++i)
                    {
                        DamageDefAdditionalHediff damageDefAdditionalHediff = additionalHediffs[i];
                        float num = ((damageDefAdditionalHediff.severityFixed <= 0f) ? (damage * damageDefAdditionalHediff.severityPerDamageDealt) : damageDefAdditionalHediff.severityFixed);
                        if (damageDefAdditionalHediff.victimSeverityScalingByInvBodySize)
                        {
                            num *= 1f / pawn.BodySize;
                        }
                        if (damageDefAdditionalHediff.victimSeverityScaling != null)
                        {
                            num *= (damageDefAdditionalHediff.inverseStatScaling ? Mathf.Max(1f - pawn.GetStatValue(damageDefAdditionalHediff.victimSeverityScaling, true, -1), 0f) : pawn.GetStatValue(damageDefAdditionalHediff.victimSeverityScaling, true, -1));
                        }
                        if (num >= 0f)
                        {
                            Hediff hediff = HediffMaker.MakeHediff(damageDefAdditionalHediff.hediff, pawn, null);
                            hediff.Severity = num;
                            pawn.health.AddHediff(hediff, null, new DamageInfo?(dinfo), null);
                        }
                        if (pawn.Dead)
                        {
                            break;
                        }
                    }
                }
            }
            return damageResult;
        }

        public FloatRange severityRange = FloatRange.Zero;
    }
}
