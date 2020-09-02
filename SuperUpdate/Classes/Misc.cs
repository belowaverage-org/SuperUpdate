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
    public static class Misc
    {
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
        public static void ReLaunch(string verb = "")
        {
            Logger.Log("Relaunching this process...");
            string args = "";
            foreach (string arg in Program.Arguments)
            {
                args += arg + " ";
            }
            Process proc = new Process();
            proc.StartInfo = new ProcessStartInfo()
            {
                FileName = Application.ExecutablePath,
                Arguments = args,
                Verb = verb,
                UseShellExecute = true
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
