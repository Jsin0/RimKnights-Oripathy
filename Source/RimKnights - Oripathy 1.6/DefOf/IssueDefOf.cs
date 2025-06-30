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
    public class IssueDefOf
    {
        static IssueDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(IssueDefOf));
        }

        public static IssueDef RK_Oripathy;
    }
}
