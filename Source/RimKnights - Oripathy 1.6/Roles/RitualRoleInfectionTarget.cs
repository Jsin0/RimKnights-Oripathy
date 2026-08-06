using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimKnights.Oripathy
{
    internal class RitualRoleInfectionTarget : RitualRole
    {
        public override bool AppliesToPawn(Pawn p, out string reason, TargetInfo selectedTarget, LordJob_Ritual ritual = null, RitualRoleAssignments assignments = null, Precept_Ritual precept = null, bool skipReason = false)
        {
            if (!base.AppliesIfChild(p, out reason, skipReason))
            {
                return false;
            }
            if (!p.RaceProps.Humanlike)
            {
                if (!skipReason)
                {
                    reason = "MessageRitualRoleMustBeHumanlike".Translate(base.LabelCap);
                }
                return false;
            }
            if (p.Ideo != null)
            {
                if (p.Ideo.PreceptsListForReading.Any((Precept pc) => pc.def.issue == IssueDefOf.RK_Oripathy))
                {
                    if (p.health != null)
                    {
                        if (!p.health.hediffSet.HasHediff(HediffDefOf.RK_Oripathy))
                        {
                            if (!p.Faction.IsPlayerSafe())
                            {
                                if (!skipReason)
                                {
                                    reason = "MessageRitualRoleMustBeColonist".Translate(base.Label);
                                }
                                return false;
                            }
                            return true;
                        }
                    }
                    if (!skipReason)
                    {
                        reason = "MessageRitualRoleMustNotHaveOripathy".Translate(base.LabelCap);
                    }
                    return false;
                }
            }
            if (!skipReason)
            {
                reason = "MessageRitualRoleMustRequireInfection".Translate(p);
            }
            return false;
        }
        public override bool AppliesToRole(Precept_Role role, out string reason, Precept_Ritual ritual = null, Pawn p = null, bool skipReason = false)
        {
            reason = null;
            return false;
        }

    }
}
