using Verse;

namespace RimKnights.Oripathy
{
    internal class Hediff_OripathyLesion : HediffWithComps
    {
        public override void PostAdd(DamageInfo? dinfo)
        {
            base.PostAdd(dinfo);
            //Guarantees that pawns with a lesion always have oripathy
            Hediff oripathy = pawn.health.GetOrAddHediff(HediffDefOf.RK_Oripathy);
            if (this.Part.depth == BodyPartDepth.Outside || this.Visible)
            {
                oripathy.SetVisible();
                this.SetVisible();
            }
        }
    }
}
