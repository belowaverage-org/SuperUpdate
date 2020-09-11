using System;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Management.Automation;
using SuperUpdate.Log;
using System.Windows.Forms;
using System.Management.Automation.Runspaces;
using System.Diagnostics;
using SuperUpdate.Classes;

namespace SuperUpdate.Engines
{
    /// <summary>
    /// This class is tasked with downloading and running the PowerShell script associated with an update.
    /// </summary>
    public class InstallEngine
    {
        /// <summary>
        /// If this property is true, Super Update will close after the PowerShell script ends.
        /// </summary>
        public static bool CloseWindowWhenDone = false;
        /// <summary>
        /// If this property is true, Super Update will re-launch after the PowerShell script ends.
        /// </summary>
        public static bool RelaunchWhenDone = false;
        /// <summary>
        /// This property stores the selected version from the UpdateSelectEngine.
        /// </summary>
        public static XmlNode SelectedVersion = null;
        private readonly PowerShell PS = PowerShell.Create();
        private readonly Runspace RS = RunspaceFactory.CreateRunspace();
        private readonly ProgressBar ProgressBar = Program.MainForm.pbMain;
        /// <summary>
        /// This constructs a new instance of InstallEngine.
        /// </summary>
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
            RS.SessionStateProxy.SetVariable("SuperUpdate", new PSRunspace());
        }
        /// <summary>
        /// This method stops the PowerShell script.
        /// </summary>
        public void Stop()
        {
            PS.Stop();
            Logger.Log("Installation canceled.", LogLevels.Information);
        }
        /// <summary>
        /// This method downloads and then runs the PowerShell script associated with the provided UpdateNode.
        /// </summary>
        /// <param name="UpdateNode">XmlNode: The "Update" node of the update to install.</param>
        /// <returns>Task: Bool: True if successful.</returns>
        public Task<bool> InstallUpdate(XmlNode UpdateNode)
        {
            SelectedVersion = UpdateNode;
            return Task.Run(async () => {
                string script = await Misc.GetFileContentURI(new Uri(UpdateNode.Attributes["ScriptURL"].Value, UriKind.RelativeOrAbsolute));
                if (script == "")
                {
                    Logger.Log("Could not retrieve script, or script is empty.", LogLevels.Warning);
                    return false;
                }
                
                PS.AddScript(script);
                Logger.Log("Starting script...");
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