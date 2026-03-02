using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using RimWorld;
using Verse;

namespace RimKnights.Oripathy
{
    [HarmonyPatch(typeof(PawnGenerator))]
    [HarmonyPatch("AddBlindness")]
    static class AddBlindness_AddOripathy
    {
        [HarmonyPostfix]
        static void Postfix(Pawn pawn)
        {
            PawnGeneration.AddOripathy(pawn);
        }
    }

    [HarmonyPatch(typeof(PawnApparelGenerator))]
    [HarmonyPatch("GenerateStartingApparelFor")]
    static class GenerateStartingApparel_GiveInfectionMonitor
    {
        [HarmonyPostfix]
        static void Postfix(Pawn pawn, PawnGenerationRequest request)
        {
            PawnGeneration.GiveInfectionMonitor(pawn, request);
        }
    }
}
