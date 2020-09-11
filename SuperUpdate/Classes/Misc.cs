using SuperUpdate.Log;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SuperUpdate.Classes
{
    /// <summary>
    /// This class contains miscellaneous methods for use in other classes.
    /// </summary>
    public static class Misc
    {
        /// <summary>
        /// Get / sets the elevation status of SuperUpdate. True means to immediatly elevate the process if it has not been elevated already.
        /// </summary>
        public static bool IsElevated { 
            get
            {
                return WindowsIdentity.GetCurrent().Groups.Contains(new SecurityIdentifier(WellKnownSidType.BuiltinAdministratorsSid, null));
            }
            set
            {
                if (value && !IsElevated)
                {
                    Logger.Log("Requesting this process be elevated...");
                    ReLaunch("RunAs");
                }
            }
        }
        /// <summary>
        /// This method re-launches SuperUpdate using the Program.Arguments, and optionally with a specified verb.
        /// </summary>
        /// <param name="verb">string: The verb to re-launch the program using (usually "RunAs").</param>
        public static void ReLaunch(string verb = "")
        {
            Logger.Log("Relaunching this process...");
            string args = "";
            foreach (string arg in Program.Arguments)
            {
                args += arg + " ";
            }
            Process proc = new Process
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = Application.ExecutablePath,
                    Arguments = args,
                    Verb = verb,
                    UseShellExecute = true
                }
            };
            try
            {
                proc.Start();
                Process.GetCurrentProcess().Kill();
            }
            catch (Exception)
            {
                Process.GetCurrentProcess().Kill();
            }
        }
        /// <summary>
        /// This method returns a string of the contents of a file from a URI object.
        /// This method supports HTTP, HTTPS, SMB (\\), direct, and absolute paths.
        /// </summary>
        /// <param name="URI">Uri: A URI object representing the file to read.</param>
        /// <returns>string: The contents of the file specified in the URI object.</returns>
        public static async Task<string> GetFileContentURI(Uri URI)
        {
            Logger.Log("Retrieving file: " + URI.ToString() + "...");
            try
            {
                if (File.Exists(URI.ToString()))
                {
                    return await Task.Run(() =>
                    {
                        return File.ReadAllText(URI.ToString());
                    });
                }
                if (URI.Scheme == "http" || URI.Scheme == "https")
                {
                    HttpClient HC = new HttpClient();
                    HttpResponseMessage msg = await HC.GetAsync(URI.ToString());
                    return await msg.Content.ReadAsStringAsync();
                }
            }
            catch (Exception e)
            {
                Logger.Log(e);
            }
            Logger.Log("File could not be retrieved...");
            return await Task.FromResult("");
        }
    }
}
