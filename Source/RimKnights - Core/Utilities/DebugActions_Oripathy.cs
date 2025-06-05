using LudeonTK;
using RimWorld;
using System;
using System.Collections.Generic;
using Verse;

namespace RimKnights
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

        [DebugAction("Originium", "Apply oripathy to all", actionType = DebugActionType.ToolMap, allowedGameStates = AllowedGameStates.PlayingOnMap)]
        public static void ApplyOripathyToAll()
        {
            Map map = Find.CurrentMap;
            if (map != null)
            {
                IReadOnlyList<Pawn> allPawnsSpawned = map.mapPawns.AllPawnsSpawned;
                for (int i = 0; i < allPawnsSpawned.Count; i++)
                {
                    allPawnsSpawned[i].health.GetOrAddHediff(HediffDefOf.RK_Oripathy);
                }
                //p.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.RK_Oripathy).Severity = 1f;
            }
        }

        [DebugAction("Originium", "shatter explosion", actionType = DebugActionType.ToolMap, allowedGameStates = AllowedGameStates.PlayingOnMap)]
        public static void DoShatterExplosion()
        {
            GenExplosion.DoExplosion(UI.MouseCell(), Find.CurrentMap, 3f, DamageDefOf.RK_ActiveOriginium, null);
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

        [DebugAction("Originium", "Test shattering all", actionType = DebugActionType.ToolMap, allowedGameStates = AllowedGameStates.PlayingOnMap)]
        public static void TestShatteringAll()
        {
            Map map = Find.CurrentMap;
            if (map != null)
            {
                IReadOnlyList<Pawn> allPawnsSpawned = map.mapPawns.AllPawnsSpawned;
                for (int i = 0; i < allPawnsSpawned.Count; i++)
                {
                    allPawnsSpawned[i].health.GetOrAddHediff(HediffDefOf.RK_Oripathy);
                    allPawnsSpawned[i].health.hediffSet.GetFirstHediffOfDef(HediffDefOf.RK_Oripathy).Severity = 1f;
                    allPawnsSpawned[i].Kill(null, null);
                }
            }
        }
    }
}
