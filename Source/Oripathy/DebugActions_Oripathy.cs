using LudeonTK;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Originium
{
    public static class DebugActions_Oripathy
    {
        [DebugAction("Originium", "Apply oripathy", actionType = DebugActionType.ToolMapForPawns, allowedGameStates = AllowedGameStates.PlayingOnMap)]
        public static void ApplyOripathy(Pawn p)
        {
            if (p != null) 
            {
                if (p.health.hediffSet.HasHediff(HediffDefOf.RK_Oripathy))
                {
                    Messages.Message(p.Name + " already has oripathy", MessageTypeDefOf.NegativeEvent);
                }
                else
                {
                    p.health.AddHediff(HediffDefOf.RK_Oripathy);
                }
                p.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.RK_Oripathy).Severity = 1f;
            }
        }

        [DebugAction("Originium", "Test shattering", actionType = DebugActionType.ToolMapForPawns, allowedGameStates = AllowedGameStates.PlayingOnMap)]
        public static void TestShattering(Pawn p)
        {
            if (p != null)
            {
                ApplyOripathy(p);
                p.Kill(null, null);
            }
        }
    }
}
