using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RimKnights.Oripathy
{
    public static class PawnGeneration
    {
        public static void AddOripathy(Pawn pawn)
        {
            if (pawn.health == null || pawn.health.hediffSet.HasHediff(HediffDefOf.RK_Oripathy))
            {
                return;
            }
            float ritualOripathyChance = GetRitualOripathyChance(pawn);
            float baseOripathyChance = OripathyMod.oripathyChance;

            //First rolls if the pawn would've gotten a ritual
            bool isRitual = ritualOripathyChance >= 0f && Rand.Chance(ritualOripathyChance);
            //Otherwise rolls if the pawn is just someone who got oripathy by chance
            bool isBaseline = !isRitual && Rand.Chance(baseOripathyChance);

            if (!isBaseline && !isRitual) return;

            List<BodyPartRecord> validParts;
            if (isRitual)
            {
                validParts = JobDriver_Infect.GetPartsToApplyOn(pawn).ToList();
            }
            else
            {
                validParts = GetValidLesionTargets(pawn).ToList();
            }

            if (validParts.Count == 0) return;

            if (isRitual) JobDriver_Infect.Infect(pawn, validParts.RandomElement<BodyPartRecord>());
            else pawn.health.GetOrAddHediff(HediffDefOf.RK_Oripathy);

            AdjustOripathyToAge(pawn);

        }
        private static IEnumerable<BodyPartRecord> GetValidLesionTargets(Pawn pawn)
        {
            foreach (BodyPartRecord bodyPartRecord in pawn.health.hediffSet.GetNotMissingParts(BodyPartHeight.Undefined, BodyPartDepth.Undefined, null, null))
            {
                if (!bodyPartRecord.def.conceptual && bodyPartRecord.def.hitPoints > 15 && bodyPartRecord.def.destroyableByDamage == true)
                {
                    yield return bodyPartRecord;
                }
            }
        }
        private static float GetRitualOripathyChance(Pawn pawn)
        {
            if (pawn.ideo == null || pawn.ideo.Ideo == null)
            {
                return -1;
            }
            return pawn.ideo.Ideo.GetOripathicPawnChance();
        }

        private static void AdjustOripathyToAge(Pawn pawn)
        {
            Hediff oripathy = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.RK_Oripathy);
            if (oripathy == null) return;

            float age = pawn.ageTracker.AgeBiologicalYearsFloat;

            //Assumes younger adults ~1 year with oripathy while older adults ~3 years with oripathy
            float yearsWithDisease = Mathf.Lerp(1f, 3f, Mathf.InverseLerp(18f, 65f, age));

            float severity = EstimateSeverity(yearsWithDisease);

            //Individual variation
            severity += Rand.Range(0.8f, 1.2f);
            //Ensures that no generated pawn has more than 30% severity
            oripathy.Severity = Mathf.Clamp(severity, 0.01f, 0.30f);

            int lesionCount = 0; //In case this needs tuning
            if (severity > 0.15f && Rand.Chance(0.30f)) lesionCount++;
            else if (severity > 0.08f && Rand.Chance(0.1f)) lesionCount++;

            for (int i = 0; i < lesionCount; i++)
            {
                List<BodyPartRecord> parts = GetValidLesionTargets(pawn).ToList();
                if (parts.Count == 0) break;
                pawn.health.AddHediff(HediffDefOf.RK_OripathyLesion, parts.RandomElement())
                    .Severity = Rand.Range(0.05f, 0.20f);
            }

        }

        private static float EstimateSeverity(float years)
        {
            //Approximation of what severity should look like after certain years
            // 1yr ≈ 0.07, 2yr ≈ 0.15, 3yr ≈ 0.24
            return new SimpleCurve
            {
                { 0f, 0.00f },
                { 1f, 0.07f },
                { 2f, 0.15f },
                { 3f, 0.24f }
            }.Evaluate(years);
        }
    }
}
