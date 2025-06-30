using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimKnights.Oripathy.Utilities
{
    public static class IdeoExtension
    {
        public static float GetOripathicPawnChance(this Ideo ideology)
        {
            if (ideology.PreceptsListForReading.NullOrEmpty<Precept>())
            {
                return 0f;
            }
            float totalChance = 0f;
            int preceptCount = 0;
            foreach (Precept precept in ideology.PreceptsListForReading)
            {
                PreceptOripathyExtension modExtention = precept.def.GetModExtension<PreceptOripathyExtension>();

                if (modExtention != null && modExtention.oripathicPawnChance >= 0f)
                {
                    totalChance += modExtention.oripathicPawnChance;
                    preceptCount++;
                }
            }
            if (preceptCount <= 0)
            {
                return 0f;
            }
            return totalChance / (float)preceptCount;
        }
    }
}
