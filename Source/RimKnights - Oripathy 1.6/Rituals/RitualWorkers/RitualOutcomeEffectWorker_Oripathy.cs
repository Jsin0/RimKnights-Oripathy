using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace RimKnights.Oripathy
{
    public class RitualOutcomeEffectWorker_Oripathy : RitualOutcomeEffectWorker_FromQuality
    {
        public RitualOutcomeEffectWorker_Oripathy() { }

        public RitualOutcomeEffectWorker_Oripathy(RitualOutcomeEffectDef def) : base(def) { }

        protected override void ApplyExtraOutcome(Dictionary<Pawn, int> totalPresence, LordJob_Ritual jobRitual, RitualOutcomePossibility outcome, out string extraOutcomeDesc, ref LookTargets letterLookTargets)
        {
            extraOutcomeDesc = null;
            if (ModsConfig.RoyaltyActive && outcome.Positive && (outcome.BestPositiveOutcome(jobRitual) || Rand.Chance(0.5f)))
            {
                Pawn pawn = ((LordJob_Ritual_Mutilation)jobRitual).mutilatedPawns[0];
                extraOutcomeDesc = "RitualOutcomeExtraDesc_OripathicPsylink".Translate(pawn.Named("PAWN"));
                List<Ability> existingAbils = pawn.abilities.AllAbilitiesForReading.ToList<Ability>();
                pawn.ChangePsylinkLevel(1, true);
                Ability ability = pawn.abilities.AllAbilitiesForReading.FirstOrDefault((Ability a) => !existingAbils.Contains(a));
                if (ability != null)
                {
                    extraOutcomeDesc += " " + "RitualOutcomeExtraDesc_OripathicPsylinkAbility".Translate(ability.def.LabelCap, pawn.Named("PAWN"));
                }
            }
        }
    }
}
