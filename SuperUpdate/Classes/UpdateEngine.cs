using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperUpdate.Log;
using SuperUpdate.Xml;
using System.Xml;

namespace SuperUpdate.Update
{
    class UpdateEngine
    {
        public static void DetectCurrentVersion()
        {
            Logger.Log("Detecting current version...", LogLevels.Information);
            foreach(XmlNode update in XmlEngine.UpdateXML.SelectNodes("/SuperUpdate/Updates/Update"))
            {
                
            }
        }
    }
}
