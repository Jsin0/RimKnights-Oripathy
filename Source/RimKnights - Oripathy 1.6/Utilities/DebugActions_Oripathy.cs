using LudeonTK;
using RimWorld;
using System;
using System.Collections.Generic;
using Verse;

namespace RimKnights.Oripathy
{
    public static class DebugActions_Oripathy
    {
        [DebugAction("Oripathy", "Apply Oripathy", actionType = DebugActionType.ToolMapForPawns, allowedGameStates = AllowedGameStates.PlayingOnMap)]
        public static void ApplyOripathy(Pawn p)
        {
            if (p != null)
            {
                if (p.health.hediffSet.HasHediff(HediffDefOf.RK_Oripathy))
                {
                    Messages.Message("MessagePawnAlreadyOripathic".Translate(p.Named("PAWN")), MessageTypeDefOf.NegativeEvent);
                }
                else
                {
                    p.health.AddHediff(HediffDefOf.RK_Oripathy);
                    Messages.Message("MessagePawnNowOripathic".Translate(p.Named("PAWN")), MessageTypeDefOf.NegativeEvent);
                }
                //p.health.GetOrAddHediff(HediffDefOf.RK_Oripathy).Severity = 1f;
                p.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.RK_Oripathy).Severity = 1f;
            }
        }

        [DebugAction("Oripathy", "Apply Oripathy to All Pawns", actionType = DebugActionType.ToolMap, allowedGameStates = AllowedGameStates.PlayingOnMap)]
        public static void ApplyOripathyToAll()
        {
            Map map = Find.CurrentMap;
            if (map != null)
            {
                IReadOnlyList<Pawn> allPawnsSpawned = map.mapPawns.AllPawnsSpawned;
                for (int i = 0; i < allPawnsSpawned.Count; i++)
                {
                    allPawnsSpawned[i].health.GetOrAddHediff(HediffDefOf.RK_Oripathy).Severity = 1f;
                }
            }
        }

        [DebugAction("Oripathy", "Do Shatter Explosion", actionType = DebugActionType.ToolMap, allowedGameStates = AllowedGameStates.PlayingOnMap)]
        public static void DoShatterExplosion()
        {
            DamageDef damageDef = DamageDefOf.RK_ActiveOriginium;
            GenExplosion.DoExplosion(UI.MouseCell(), Find.CurrentMap, 3f, damageDef, null);
        }

        [DebugAction("Oripathy", "Start Shattering", actionType = DebugActionType.ToolMapForPawns, allowedGameStates = AllowedGameStates.PlayingOnMap)]
        public static void TestShattering(Pawn p)
        {
            if (p != null)
            {
                ApplyOripathy(p);
                p.Kill(null, null);
            }
        }

        [DebugAction("Oripathy", "Start Shattering on All Pawns", actionType = DebugActionType.ToolMap, allowedGameStates = AllowedGameStates.PlayingOnMap)]
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
