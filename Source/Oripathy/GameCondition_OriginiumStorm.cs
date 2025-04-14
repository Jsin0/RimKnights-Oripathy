using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace Originium
{
    public class GameCondition_OriginiumStorm : GameCondition_OriginiumRain
    {

        private void DoPawnsToxicDamage(Map map)
        {
            IReadOnlyList<Pawn> allPawnsSpawned = map.mapPawns.AllPawnsSpawned;
            for (int i = 0; i < allPawnsSpawned.Count; i++)
            {
                if (!allPawnsSpawned[i].kindDef.immuneToGameConditionEffects)
                {
                    GameCondition_OriginiumRain.DoPawnToxicDamage(allPawnsSpawned[i], true, false, 2f);
                }
            }
        }

        public override SkyTarget? SkyTarget(Map map)
        {
            return new SkyTarget?(new SkyTarget(0.85f, this.OriginiumRainColors, 1f, 1f));
        }

        private SkyColorSet OriginiumRainColors = new SkyColorSet(new ColorInt(204, 176, 91).ToColor, new ColorInt(212, 117, 0).ToColor, new Color(0.4f, 0.4f, 0.4f), 0.85f);

        private List<SkyOverlay> overlays = new List<SkyOverlay>();
    }
}
