using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace Originium.Utilities
{
    [StaticConstructorOnStartup]
    static class StringPatcher
    {
        static StringPatcher()
        {
            if(OriMod.settings.orifuel || OriMod.settings.orundum)
            {
                PatchText();
            }
        }
        public static void PatchText()
        {
            IEnumerable<Type> allDefTypes = typeof(Def).AllSubclassesNonAbstract();
            List<(string, string)> pairs = GenerateStringPairs();
            foreach (Type type in allDefTypes)
            {
                foreach(Def def in GenDefDatabase.GetAllDefsInDatabaseForDef(type))
                {
                    bool flag = false;
                    if(def?.label != null)
                    {
                        foreach((string,string) pair in pairs)
                        {
                            if (def.label.Contains(pair.Item1))
                            {
                                //Log.Message("replacing label");
                                def.label = def.label.Replace(pair.Item1, pair.Item2);
                                flag = true;
                            }
                        }
                    }
                    if(def?.description != null)
                    {
                        foreach ((string, string) pair in pairs)
                        {
                            if (def.description.Contains(pair.Item1))
                            {
                                //Log.Message("replacing description");
                                def.description = def.description.Replace(pair.Item1, pair.Item2);
                                flag = true;
                            }

                        }
                    }
                    if (flag) Log.Message(def.label + ": " + def.description);
                }
            }
        }

        static List<(string,string)> GenerateStringPairs()
        {
            List<(string,string)> pairs = new List<(string,string)> ();
            if (OriMod.settings.orifuel)
            {
                pairs.Add(("chemfuel", "orifuel"));
                if(ModLister.GetActiveModWithIdentifier("vanillaexpanded.vchemfuele") != null)
                {
                    pairs.Add(("deepchem", "crude originium"));
                }
            }
            if (OriMod.settings.orundum)
            {
                pairs.Add(("silver", "orundum"));
            }
            //Log.Message(pairs);
            return pairs;
        }
    }
}
