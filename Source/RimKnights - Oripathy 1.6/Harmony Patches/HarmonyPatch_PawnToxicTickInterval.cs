using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Verse;

namespace RimKnights.Oripathy
{
    [HarmonyPatch(typeof(ToxicUtility))]
    [HarmonyPatch("PawnToxicTickInterval")]
    static class HarmonyPatch_PawnToxicTickInterval
    {
        [HarmonyPostfix]
        static void Postfix(Pawn pawn, int delta)
        {
            if (!pawn.IsHashIntervalTick(3451, delta) || !pawn.Spawned)
            {
                return;
            }
            float multiplier = pawn.Position.GetTerrain(pawn.Map).toxicBuildupFactor;
            if (ModsConfig.BiotechActive && pawn.Position.IsPolluted(pawn.Map))
            {
                multiplier += 1f;
            }
            if (multiplier > 0f)
            {
                const float severityPerDay = 0.6f;
                const float severityPerActiveTick = severityPerDay / Utilities.Constants.TicksPerDay * 3451;
                OripathyUtility.DoPawnOriginiumDamage(pawn, severityPerActiveTick, multiplier);
            }
        }
    }
}
