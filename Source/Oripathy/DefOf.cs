using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace Oripathy
{
    [DefOf]
    public static class DamageDefOf
    {
        static DamageDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(DamageDefOf));
        }
    public static DamageDef OriginiumDust;
    }
}
