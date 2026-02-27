using System;
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace RimKnights.Oripathy
{
    public class HediffCompProperties_DynamicSeverityPerDay : HediffCompProperties
    {
        public HediffCompProperties_DynamicSeverityPerDay()
        {
            this.compClass = typeof(HediffComp_DynamicSeverityPerDay);
        }

        public List<AffectorHediff> AffectorHediffs;

        public bool cumulative = false;

        public float mechanitorFactor = 1f;

        public float minAge = 0f;
        public override IEnumerable<string> ConfigErrors(HediffDef parentDef)
        {
            foreach(string text in base.ConfigErrors(parentDef))
            {
                yield return text;
            }
            if (AffectorHediffs.NullOrEmpty())
            {
                yield return $"{parentDef.defName}: HediffComp_DynamicSeverrityPerDay has no AffectorHediffs defined";
            }
            else
            {
                for (int i = 0; i < AffectorHediffs.Count; i++)
                {
                    AffectorHediff hediff = AffectorHediffs[i];
                    if (hediff == null)
                    {
                        yield return $"{parentDef.defName}: AffectorHediffs[{i}] is null";
                    }
                    else if (hediff.hediff == null)
                    {
                        yield return $"{parentDef.defName}: AffectorHediffs[{i}] has no hediff specified";
                    } else if (hediff.curve == null && hediff.severityFactor == 0)
                    {
                        yield return $"{parentDef.defName}: AffectorHediffs[{i}] / {hediff.hediff.defName} : curve and factor are not defined. No severity adjustments will be made.";
                    }
                }
            }

        }

    }
}
