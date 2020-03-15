using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Net.Http;
using SuperUpdate.Log;

namespace SuperUpdate.Xml
{
    class XmlEngine
    {
        private static HttpClient HttpClient = new HttpClient();
        public async static void ReadRemoteXML(string XmlURL)
        {
            try
            {
                Logger.Log("Retrieving list of updates...", LogLevels.Information);
                Logger.Log("Reading URL: " + XmlURL + "...");
                HttpResponseMessage message = await HttpClient.GetAsync(XmlURL);
                string XML = await message.Content.ReadAsStringAsync();
                Logger.Log(XML);
            }
            catch (Exception e)
            {
                Logger.Log("Could not retrieve list of updates!", e);
            }
        }
    }
}
