using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Oripathy
{
    public class ShatterManager : MapComponent
    {
        public ShatterManager (Map map) : base(map)
        {
        }
        public void Register( Hediff_Oripathy o)
        {
            this.oripathicCorpses.Add(o);
        }
        public void Deregister(Hediff_Oripathy o)
        {
            this.oripathicCorpses.Remove(o);
        }
        public override void MapComponentTick()
        {
            if (Find.TickManager.TicksGame % 250 == 0)
            {
                for (int i = 0; i < this.oripathicCorpses.Count; i++)
                {
                    this.oripathicCorpses[i].tryShatter();
                }
            }
        private List<Hediff_Oripathy> oripathicCorpses = new List<Hediff_Oripathy>();


    }
}
