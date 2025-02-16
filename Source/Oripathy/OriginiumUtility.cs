using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Originium
{
    public class OriginiumUtility
    {
        public static bool ExposedToOriginium(Pawn pawn)
        {
            if (pawn.MapHeld == null)
            {
                return false;
            }
            GameCondition_OriginiumRain activeCondition = pawn.MapHeld.gameConditionManager.GetActiveCondition<GameCondition_OriginiumRain>();

            return activeCondition != null && activeCondition.HiddenByOtherCondition(pawn.MapHeld) && (activeCondition.HiddenByOtherCondition(pawn.MapHeld)) && (activeCondition.TicksPassed > 2000 && (pawn.Map != null) && !pawn.Position.Roofed(pawn.Map));
        }
    }
}
