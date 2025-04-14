using RimWorld;
using System;
using UnityEngine;
using Verse;

namespace Originium
{
    public class Hediff_OriginiumBuildup : HediffWithComps
    {
        public override string SeverityLabel
        {
            get
            {
                if (this.Severity <= 0f)
                {
                    return null;
                }
                return this.Severity.ToStringPercent("F0");
            }
        }
        public override void PostAdd(DamageInfo? dinfo)
        {
            if (!this.pawn.RaceProps.IsFlesh)
            {
                this.pawn.health.RemoveHediff(this);
                return;
            }
            else if (GeneUtility.IsBaseliner(this.pawn) && OripathyMod.settings.baselinersImmune)
            {
                this.pawn.health.RemoveHediff(this);
                return;
            }

            base.PostAdd(dinfo);
        }
    }
}
