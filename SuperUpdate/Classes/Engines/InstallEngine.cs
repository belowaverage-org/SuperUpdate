using System;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Management.Automation;
using SuperUpdate.Log;
using System.Net.Http;
using System.Windows.Forms;
using System.Management.Automation.Runspaces;
using System.Diagnostics;
using SuperUpdate.Classes;

namespace SuperUpdate.Engines
{
    public class InstallEngine
    {
        public static bool CloseWindowWhenDone = false;
        public static bool RelaunchWhenDone = false;
        private PowerShell PS = PowerShell.Create();
        private Runspace RS = RunspaceFactory.CreateRunspace();
        private HttpClient HC = new HttpClient();
        private ProgressBar ProgressBar = Program.MainForm.pbMain;
        public InstallEngine()
        {
            PS.Runspace = RS;
            RS.Open();
            PS.Streams.Debug.DataAdded += Debug_DataAdded;
            PS.Streams.Error.DataAdded += Error_DataAdded;
            PS.Streams.Warning.DataAdded += Warning_DataAdded;
            PS.Streams.Verbose.DataAdded += Verbose_DataAdded;
            PS.Streams.Information.DataAdded += Information_DataAdded;
            PS.Streams.Progress.DataAdded += Progress_DataAdded;
            RS.SessionStateProxy.SetVariable("SuperUpdate", new Classes.PSRunspace());
        }
        public void Stop()
        {
            PS.Stop();
            Logger.Log("Installation canceled.", LogLevels.Information);
        }
        public Task<bool> InstallUpdate(XmlNode UpdateNode)
        {
            Logger.Log("Downloading installation script...");
            return Task.Run(async () => {
                HttpResponseMessage msg = await HC.GetAsync(UpdateNode.Attributes["ScriptURL"].Value);
                string script = await msg.Content.ReadAsStringAsync();
                PS.AddScript(script);
                PS.Invoke();
                Logger.Log("Done!");
                if (RelaunchWhenDone) Misc.ReLaunch();
                if (CloseWindowWhenDone || RelaunchWhenDone) Process.GetCurrentProcess().Kill();
                return true;
            });
        }
        private void Progress_DataAdded(object sender, DataAddedEventArgs e)
        {
            ProgressRecord progressRecord = PS.Streams.Progress.Last();
            Logger.Log("Progress: " + progressRecord.Activity + ": " + progressRecord.StatusDescription + ": " + progressRecord.PercentComplete + "%", LogLevels.Verbose);
            Program.MainForm.Invoke(new Action(() => {
                if (progressRecord.PercentComplete > 100 || progressRecord.PercentComplete < 0)
                {
                    ProgressBar.Style = ProgressBarStyle.Marquee;
                }
                else
                {
                    ProgressBar.Style = ProgressBarStyle.Continuous;
                    ProgressBar.Value = progressRecord.PercentComplete;
                }
            }));
        }
        private void Information_DataAdded(object sender, DataAddedEventArgs e)
        {
            InformationRecord informationRecord = PS.Streams.Information.Last();
            Logger.Log(informationRecord.MessageData.ToString(), LogLevels.Information);
        }
        private void Verbose_DataAdded(object sender, DataAddedEventArgs e)
        {
            VerboseRecord verboseRecord = PS.Streams.Verbose.Last();
            Logger.Log(verboseRecord.Message, LogLevels.Verbose);
        }
        private void Warning_DataAdded(object sender, DataAddedEventArgs e)
        {
            WarningRecord warningRecord = PS.Streams.Warning.Last();
            Logger.Log(warningRecord.Message, LogLevels.Warning);
        }
        private void Error_DataAdded(object sender, DataAddedEventArgs e)
        {
            ErrorRecord errorRecord = PS.Streams.Error.Last();
            Logger.Log(errorRecord.Exception);
            Logger.Log(errorRecord.ScriptStackTrace, LogLevels.Exception);
        }
        private void Debug_DataAdded(object sender, DataAddedEventArgs e)
        {
            DebugRecord debugRecord = PS.Streams.Debug.Last();
            Logger.Log("DEBUG: " + debugRecord.Message, LogLevels.Verbose);
        }
    }
}
