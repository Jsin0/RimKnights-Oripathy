using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace RimKnights.Oripathy
{
    public class JobGiver_Infect : ThinkNode_JobGiver
    {
        protected override Job TryGiveJob(Pawn pawn)
        {
            Lord lord = pawn.GetLord();
            LordJob_Ritual_Mutilation lordJob_Ritual_Mutilation;
            if (lord == null || (lordJob_Ritual_Mutilation = lord.LordJob as LordJob_Ritual_Mutilation) == null)
            {
                return null;
            }
            Pawn pawn2 = pawn.mindState.duty.focusSecond.Pawn;
            if (lordJob_Ritual_Mutilation.mutilatedPawns.Contains(pawn2) || !pawn.CanReserveAndReach(pawn2, PathEndMode.ClosestTouch, Danger.None, 1, -1, null, false))
            {
                return null;
            }
            return JobMaker.MakeJob(JobDefOf.RK_Infect, pawn2, pawn.mindState.duty.focus);
        }
    }
}
