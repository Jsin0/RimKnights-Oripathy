using RimWorld;
using System;
using UnityEngine;
using Verse;

namespace RimKnights
{
    public class Hediff_OriginiumBuildup : HediffVisibleWithApparel
    {
        public override bool ShouldRemove
        {
            get 
            { 
                return base.ShouldRemove && !this.isOripathic; 
            }
        }

        public override void PostAdd(DamageInfo? dinfo)
        {
            if (!this.pawn.RaceProps.IsFlesh)
            {
                this.pawn.health.RemoveHediff(this);
                return;
            }
            else if (GeneUtility.IsBaseliner(this.pawn) && CoreMod.settings.baselinersImmune)
            {
                this.pawn.health.RemoveHediff(this);
                return;
            }

            base.PostAdd(dinfo);
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<bool>(ref this.isOripathic, "isOripathic", false);
        }

        public bool isOripathic = false;
    
    }

}
