using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace Oripathy
{
    public class ShatterManager : MapComponent
    {
        public ShatterManager (Map map) : base(map)
        {
        }
        public void Register( Corpse corpse)
        {
            this.oripathicCorpses.Add(corpse);
        }
        public void Deregister(Corpse corpse)
        {
            this.oripathicCorpses.Remove(corpse);
        }
        public override void MapComponentTick()
        {
            if (Find.TickManager.TicksGame % 250 == 0)
            {
                for (int i = 0; i < this.oripathicCorpses.Count; i++)
                {
                    TryTriggerCountdownShatter(this.oripathicCorpses[i]);
                }
            }
        }

        private void TryTriggerCountdownShatter(Corpse corpse)
        {
            corpse.InnerPawn.health.hediffSet.HasHediff()
        }
        private List<Corpse> oripathicCorpses = new List<Corpse>();


    }
}
