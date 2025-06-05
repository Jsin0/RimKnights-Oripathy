using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Verse;

namespace RimKnights.Utilities
{
    internal class PatchOperationModConditional : PatchOperationPathed
    {
        private string settingName;
        private PatchOperation match;

        protected override bool ApplyWorker(XmlDocument xml)
        {
            var field = typeof(CoreModSettings).GetField(settingName);
            if(field == null)
            {
                Log.Message("null field");
                return false;
            }
            bool active = (bool)field.GetValue(CoreMod.settings);
            if( active )
            {
                return this.match.Apply(xml);
            }
            else
            {
                return true;
            }
        }

    }
}
