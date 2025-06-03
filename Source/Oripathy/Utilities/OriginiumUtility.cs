using System;




using Verse;

namespace RimKnights
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
