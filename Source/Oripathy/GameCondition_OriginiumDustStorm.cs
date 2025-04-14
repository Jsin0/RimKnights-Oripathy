using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace Originium
{
    public class GameCondition_OriginiumDustStorm : GameCondition_OriginiumRain
    {
        private void DoPawnsToxicDamage(Map map)
        {
            IReadOnlyList<Pawn> allPawnsSpawned = map.mapPawns.AllPawnsSpawned;
            for (int i = 0; i < allPawnsSpawned.Count; i++)
            {
                if (!allPawnsSpawned[i].kindDef.immuneToGameConditionEffects)
                {
                    GameCondition_OriginiumRain.DoPawnToxicDamage(allPawnsSpawned[i], false, true);
                }
            }
        }
    }
}
