using System;
using System.Diagnostics;
using System.Security.Principal;
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
                        Verb = "RunAs",
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
            }
        }
    }
}
