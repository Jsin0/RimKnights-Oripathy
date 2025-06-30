using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace RimKnights.Oripathy
{
    [DefOf]
    public static class HistoryEventDefOf
    {
        static HistoryEventDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(HistoryEventDefOf));
        }

        public static HistoryEventDef RK_BecameOripathic;

        [MayRequireIdeology]
        public static HistoryEventDef RK_BecameOripathic_Ritual;

        [MayRequireIdeology]
        public static HistoryEventDef RK_RemovedLesion;
    }
}
