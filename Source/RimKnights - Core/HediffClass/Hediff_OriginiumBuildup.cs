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


        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<bool>(ref this.isOripathic, "isOripathic", false);
        }

        public bool isOripathic = false;
    
    }

}
