using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using UnityEngine;
using Verse;

namespace RimKnights.Oripathy
{
    public static class PawnGeneration
    {
        public static void AddOripathy(Pawn pawn)
        {
            if (pawn.health == null || !pawn.RaceProps.IsFlesh || pawn.health.hediffSet.HasHediff(HediffDefOf.RK_Oripathy))
            {
                return;
            }
            bool isAnimal = false;
            bool isRitual = false;
            bool isBaseline = false;

            if (pawn.RaceProps.Humanlike)
            {
                float ritualOripathyChance = GetRitualOripathyChance(pawn);
                float baseOripathyChance = OripathyMod.settings.oripathyChance;

                //First rolls if the pawn would've gotten a ritual
                isRitual = ritualOripathyChance >= 0f && Rand.Chance(ritualOripathyChance);
                //Otherwise rolls if the pawn is just someone who got oripathy by chance
                isBaseline = !isRitual && Rand.Chance(baseOripathyChance);
            }
            else
            {
                const float animalOripathyChance = 0.005f;

                isAnimal = Rand.Chance(animalOripathyChance);
            }

            if (!isAnimal && !isBaseline && !isRitual) return;

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

            if(!isAnimal) AdjustOripathyToAge(pawn);

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

            //Assumes younger adults ~0.2 year with oripathy while older adults ~3 years with oripathy
            float yearsWithDisease = Mathf.Lerp(0.2f, 3f, Mathf.InverseLerp(18f, 65f, age));

            float severity = EstimateSeverity(yearsWithDisease);

            //Individual variation
            severity *= Rand.Range(0.8f, 1.2f);
            //Ensures that no generated pawn has more than 25% severity
            severity = Mathf.Clamp(severity, 0.001f, 0.25f);
            oripathy.Severity = severity;

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

            ((Hediff_OriginiumBase)oripathy).RefreshDisplayedSeverity();
        }

        private static float EstimateSeverity(float years)
        {
            //Approximation of what severity should look like after certain years
            // 1yr ≈ 0.07, 2yr ≈ 0.15, 3yr ≈ 0.24
            return new SimpleCurve
            {
                { 0f, 0.001f },
                { 1f, 0.07f },
                { 2f, 0.15f },
                { 3f, 0.24f }
            }.Evaluate(years);
        }

        public static void GiveInfectionMonitor(Pawn pawn, PawnGenerationRequest request)
        {
            if (!OripathyMod.settings.infectionMonitor || !pawn.RaceProps.ToolUser || !pawn.RaceProps.IsFlesh || pawn.RaceProps.IsAnomalyEntity || pawn.Faction == null)
            {
                return;
            }
            if (pawn.health == null || !pawn.health.hediffSet.HasHediff(HediffDefOf.RK_Oripathy))
            {
                return;
            }

            float chance = 0f;
            //Higher tech factions have higher chance of having an infection monitor and favor having the implant over the apparel
            //Chance is reused as both the chance to get something to measure oripathy and the chance that thing is an implant
            switch (pawn.Faction.def.techLevel)
            {
                case TechLevel.Archotech:
                    chance = 1f;
                    break;
                case TechLevel.Ultra:
                    chance = 0.80f;
                    break;
                case TechLevel.Spacer:
                    chance = 0.65f;
                    break;
                case TechLevel.Industrial:
                    chance = 0.25f;
                    break;
            }

            if (Rand.Chance(chance))
            {
                if (!Rand.Chance(chance))
                {
                    Apparel apparel = (Apparel)ThingMaker.MakeThing(ThingDefOf.RK_InfectionMonitor);
                    if (pawn.apparel.CanWearWithoutDroppingAnything(apparel.def))
                    {
                        pawn.apparel.Wear(apparel, false);
                        return;
                    }
                }

                BodyPartRecord bodyPartTorso = pawn?.RaceProps?.body?.AllParts.FirstOrDefault(p => p.def == BodyPartDefOf.Torso);
                if (bodyPartTorso != null && !pawn.health.hediffSet.HasHediff(HediffDefOf.RK_InfectionMonitorImplant))
                {
                    pawn.health.GetOrAddHediff(HediffDefOf.RK_InfectionMonitorImplant, bodyPartTorso);
                }
            }


        }
    }
}
