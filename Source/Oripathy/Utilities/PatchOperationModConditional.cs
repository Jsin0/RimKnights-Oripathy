using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Verse;

namespace Originium.Utilities
{
    internal class PatchOperationModConditional : PatchOperationPathed
    {
        public string settingName;
        private PatchOperation match;

        protected override bool ApplyWorker(XmlDocument xml)
        {
            var field = typeof(OripathyModSettings).GetField(settingName);
            if(field == null)
            {
                Log.Message("null field");
                return false;
            }
            bool active = (bool)field.GetValue(OriMod.settings);
            if( active )
            {
                return this.match.Apply(xml);
            }
            else
            {
                Log.Message("setting inactive");
                return active;
            }
        }

    }
}
