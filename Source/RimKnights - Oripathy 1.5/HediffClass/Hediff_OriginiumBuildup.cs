using RimWorld;
using System;
using UnityEngine;
using Verse;

namespace RimKnights.Oripathy
{
    public class Hediff_OriginiumBuildup : Hediff_OriginiumBase
    {
        public override bool ShouldRemove
        {
            get 
            { 
                return !this.isOripathic && base.ShouldRemove; 
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
