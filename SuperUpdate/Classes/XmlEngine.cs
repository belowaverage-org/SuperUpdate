using System;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using SuperUpdate.Log;
using System.IO;

namespace SuperUpdate.Xml
{
    class XmlEngine
    {
        public static int MaxRedirects = 10;
        public static XmlDocument UpdateXML = null;
        public static Task<bool> ReadXML(string URI)
        {
            return Task.Run(async () => {
                try
                {
                    bool result = true;
                    UpdateXML = new XmlDocument();
                    Logger.Log("Retrieving list of updates...", LogLevels.Information);
                    Logger.Log("Reading XML: " + URI + "...");
                    StringReader schemaReader = new StringReader(Properties.Resources.UpdateSchema);
                    UpdateXML.Schemas.Add(XmlSchema.Read(schemaReader, XmlThrowHere));
                    UpdateXML.Load(URI);
                    Logger.Log("Validating Update XML...");
                    UpdateXML.Validate(XmlThrowHere);
                    Logger.Log("Update XML has been read and validated!");
                    XmlNode redirect = UpdateXML.SelectSingleNode("/SuperUpdate/Settings/Redirect");
                    if (redirect != null)
                    {
                        Logger.Log("XML Redirect Found! Redirecting...");
                        if (RedirectCount++ >= MaxRedirects)
                        {
                            Logger.Log("Followed too many redirects! The limit is: " + MaxRedirects + ".", LogLevels.Warning);
                        }
                        else
                        {
                            result = await ReadXML(redirect.Attributes["RedirectURL"].Value);
                        }
                    }
                    RedirectCount = 0;
                    return result;
                }
                catch (XmlSchemaException e)
                {
                    Logger.Log("Not a valid Super Update XML!", e);
                    return false;
                }
                catch (Exception e)
                {
                    Logger.Log("Could not retrieve list of updates!", e);
                    return false;
                }
            });
        }
        private static int RedirectCount = 0;
        private static void XmlThrowHere(object Sender, ValidationEventArgs Event)
        {
            throw Event.Exception;
        }
    }
}
