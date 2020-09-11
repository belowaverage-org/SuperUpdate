using System;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using SuperUpdate.Log;
using SuperUpdate.Classes;
using System.IO;
using System.Drawing;
using System.Net.Http;

namespace SuperUpdate.Engines
{
    /// <summary>
    /// This class is tasked with parsing the XML passed to Super Update.
    /// </summary>
    public class XmlEngine
    {
        /// <summary>
        /// The maximum times the XML engine is allowed to follow a redirect.
        /// </summary>
        public static int MaxRedirects = 100;
        /// <summary>
        /// The XML document pulled from the ReadXML method.
        /// </summary>
        public static XmlDocument UpdateXML = null;
        /// <summary>
        /// The XML Namespace Manager that contains the namespace information for Super Update.
        /// </summary>
        public static XmlNamespaceManager XNS = null;
        private static int RedirectCount = 0;
        private static readonly HttpClient HttpClient = new HttpClient();
        /// <summary>
        /// This method will read the XML at this URI: HTTP, HTTPS, \\, absolute, and relative paths are supported.
        /// </summary>
        /// <param name="URI">The URI string of the XML to parse.</param>
        /// <returns>Task: Bool: Returns true if successful.</returns>
        public static Task<bool> ReadXML(string URI)
        {
            return Task.Run(async () => {
                try
                {
                    bool result = true;
                    UpdateXML = new XmlDocument();
                    XNS = new XmlNamespaceManager(UpdateXML.NameTable);
                    XNS.AddNamespace("SU", "http://belowaverage.org/schemas/superupdate/0.0.0.2");
                    Logger.Log("Retrieving list of updates...", LogLevels.Information);
                    Logger.Log("Reading XML: " + URI + "...");
                    StringReader schemaReader = new StringReader(Properties.Resources.UpdateSchema);
                    UpdateXML.Schemas.Add(XmlSchema.Read(schemaReader, XmlThrowHere));
                    UpdateXML.Load(URI);
                    Logger.Log("Validating Update XML...");
                    UpdateXML.Validate(XmlThrowHere);
                    Logger.Log("Update XML has been read and validated!");
                    ParseSettings();
                    XmlNode redirect = UpdateXML.SelectSingleNode("/SU:SuperUpdate/SU:Settings/SU:Redirect", XNS);
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
        private static void XmlThrowHere(object Sender, ValidationEventArgs Event)
        {
            throw Event.Exception;
        }
        private static void ParseSettings()
        {
            if (Program.MainForm.InvokeRequired)
            {
                Program.MainForm.Invoke(ParseSettingsUI);
            }
            else
            {
                ParseSettingsUI();
            }
        }
        private static readonly Action ParseSettingsUI = new Action(async () => {
            Main main = Program.MainForm;
            foreach (XmlNode setting in UpdateXML.SelectNodes("/SU:SuperUpdate/SU:Settings/SU:*", XNS))
            {
                if (setting.Name == "WindowTitle")
                {
                    main.Text = setting.Attributes["Title"].Value;
                    continue;
                }
                if (setting.Name == "WindowSize")
                {
                    string size = setting.Attributes["Size"].Value;
                    if (size == "Expanded") main.Size = main.ExpandedSize;
                    if (size == "Contracted") main.Size = main.MinimumSize;
                    string[] widthHeight = size.Split('x');
                    if (widthHeight.Length == 2 && size != "Expanded")
                    {
                        main.Size = new Size(int.Parse(widthHeight[0]), int.Parse(widthHeight[1]));
                    }
                    main.CenterWindow();
                    continue;
                }
                if (setting.Name == "WindowIcon")
                {
                    HttpResponseMessage message = await HttpClient.GetAsync(new Uri(setting.Attributes["URL"].Value));
                    Stream stream = await message.Content.ReadAsStreamAsync();
                    main.Icon = new Icon(stream);
                    stream.Close();
                    message.Dispose();
                    continue;
                }
                if (setting.Name == "WindowIconLarge")
                {
                    main.LargeImageStatic = setting.Attributes["URL"].Value;
                    main.RefreshLargeIcon();
                    continue;
                }
                if (setting.Name == "WindowIconLargeAnimated")
                {
                    main.LargeImageSpinner = setting.Attributes["URL"].Value;
                    main.RefreshLargeIcon();
                    continue;
                }
                if (setting.Name == "RequireElevation")
                {
                    Misc.IsElevated = bool.Parse(setting.Attributes["Value"].Value);
                    continue;
                }
                if (setting.Name == "AutoRun")
                {
                    Program.MainForm.AutoRun = bool.Parse(setting.Attributes["Value"].Value);
                    continue;
                }
            }
        });
    }
}
