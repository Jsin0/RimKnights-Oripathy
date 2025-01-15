using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace Oripathy
{
    [StaticConstructorOnStartup]
    public static class Startup
    {
        static Startup()
        {
            Log.Message("Oripathy loaded.");
        }
    }
}
