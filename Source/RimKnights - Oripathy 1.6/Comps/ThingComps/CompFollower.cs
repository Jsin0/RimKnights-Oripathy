using Verse;
using RimWorld;

namespace RimKnights.Oripathy
{
    public class CompFollower : ThingComp
    {
        public CompProperties_Follower Props
        {
            get
            {
                return (CompProperties_Follower)this.props;
            }
        }

        public override void CompTick()
        {
            base.CompTick();
            if (currentTarget == null || currentTarget.Destroyed)
            {
                parent.Destroy();
                return;
            }

            if (!parent.IsHashIntervalTick(30)) return;

            if (currentTarget.Spawned)
            {
                if(parent.Position != currentTarget.Position)
                {
                    parent.Position = currentTarget.Position;
                }
            }else if(currentTarget.ParentHolder is Pawn_CarryTracker carrier)
            {
                if(parent.Position != carrier.pawn.Position)
                {
                    parent.Position = carrier.pawn.Position;
                }
            }
            else
            {
                parent.Destroy();
                return;
            }
        }

        public void SetTarget(Thing target)
        {
            if (target == null)
            {
                Log.Warning("CompFollowerNull".Translate());
                return;
            }

            currentTarget = target;
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_References.Look(ref currentTarget, "currentTarget");
        }

        private Thing currentTarget;
    }
}
