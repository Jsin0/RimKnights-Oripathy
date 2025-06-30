using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;

namespace RimKnights.Oripathy
{
    [DefOf]
    public static class RecipeDefOf
    {
        static RecipeDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(RecipeDefOf));
        }

        public static RecipeDef RK_ExciseLesion;

        public static RecipeDef RK_HarvestLesion;
    }
}
