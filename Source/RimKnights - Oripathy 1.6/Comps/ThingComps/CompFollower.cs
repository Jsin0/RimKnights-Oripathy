using Verse;

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
            if (followTarget == null || followTarget.Destroyed || !followTarget.Spawned)
            {
                this.parent.Destroy();
            }

            if (followTarget.Spawned)
            {
                if(parent.Position != followTarget.Position)
                {
                    this.parent.Position = followTarget.Position;
                }
            }
        }

        public void SetTarget(Thing target)
        {
            if (target == null)
            {
                Log.Warning("CompFollowerNull".Translate());
                return;
            }

            followTarget = target;
        }

        private Thing followTarget;
    }
}
