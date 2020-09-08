using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml;
using SuperUpdate.Engines;
using SuperUpdate.Log;

namespace SuperUpdate.Classes
{
    public class PSRunspace
    {
        /// <summary>
        /// Get / set the window state of the main window.
        /// </summary>
        public FormWindowState WindowState
        {
            get
            {
                return Program.MainForm.WindowState;
            }
            set
            {
                Program.MainForm.Invoke(new Action(() => {
                    Program.MainForm.WindowState = value;
                }));
            }
        }
        /// <summary>
        /// Get / set the title of the main window.
        /// </summary>
        public string WindowText
        {
            get
            {
                return Program.MainForm.Text;
            }
            set
            {
                Program.MainForm.Invoke(new Action(() => {
                    Program.MainForm.Text = value;
                }));
            }
        }
        /// <summary>
        /// Gets / sets the visibility status of the main window. Setting this false will hide the main window.
        /// </summary>
        public bool WindowVisible
        {
            get
            {
                return Program.MainForm.Visible;
            }
            set
            {
                Program.MainForm.Invoke(new Action(() => {
                    Program.MainForm.Visible = value;
                }));
            }
        }
        /// <summary>
        /// Gets / sets the expansion status of the main window. True means that the main window is expanded.
        /// </summary>
        public bool WindowExpanded
        {
            get
            {
                return Program.MainForm.Expanded;
            }
            set
            {
                Program.MainForm.Invoke(new Action(() => {
                    if (!Program.MainForm.Expanded && value) Program.MainForm.ExpandContract();
                    if (Program.MainForm.Expanded && !value) Program.MainForm.ExpandContract();
                }));
            }
        }
        /// <summary>
        /// Get / set this variable to specify whether or not to close SuperUpdate once the PowerShell script finishes.
        /// </summary>
        public bool CloseWindowWhenDone
        {
            get
            {
                return InstallEngine.CloseWindowWhenDone;
            }
            set
            {
                InstallEngine.CloseWindowWhenDone = value;
            }
        }
        /// <summary>
        /// Get / set this variable to specify whether or not to re-launch SuperUpdate once the PowerShell script finishes.
        /// </summary>
        public bool RelaunchWhenDone
        {
            get
            {
                return InstallEngine.RelaunchWhenDone;
            }
            set
            {
                InstallEngine.RelaunchWhenDone = value;
            }
        }
        /// <summary>
        /// Gets / sets the elevation status of SuperUpdate. Setting this true will immediatly elevate the process.
        /// </summary>
        public bool Elevated
        {
            get
            {
                return Misc.IsElevated;
            }
            set
            {
                Misc.IsElevated = value;
            }
        }
        /// <summary>
        /// Gets / sets the SuperUpdateArguments that were passed to SuperUpdate on launch.
        /// </summary>
        public string[] SuperUpdateArguments
        {
            get
            {
                return Program.Arguments;
            }
            set
            {
                Program.Arguments = value;
            }
        }
        /// <summary>
        /// Gets the arguments passed to the PowerShell script via the update XML node.
        /// </summary>
        public string[] ScriptArguments
        {
            get
            {
                if (InstallEngine.SelectedVersion.Attributes["ScriptArguments"] != null)
                {
                    return InstallEngine.SelectedVersion.Attributes["ScriptArguments"].Value.Split(' ');
                }
                return new string[0];
            }
        }
        /// <summary>
        /// Gets the currently detected version of the software to be updated.
        /// </summary>
        public XmlNode CurrentVersion
        {
            get
            {
                return UpdateEngine.CurrentVersion;
            }
        }
        /// <summary>
        /// Gets the latest possible version of the software to be updated.
        /// </summary>
        public XmlNode LatestVersion
        {
            get
            {
                return UpdateEngine.LatestVersion;
            }
        }
        /// <summary>
        /// Gets the selected update version that the user selected on the updates screen.
        /// </summary>
        public XmlNode SelectedVersion
        {
            get
            {
                return InstallEngine.SelectedVersion;
            }
        }
        /// <summary>
        /// Gets a list of log items in current log.
        /// </summary>
        public List<LogItem> LogItems
        {
            get
            {
                return Logger.LogItems;
            }
        }
        /// <summary>
        /// Gets the SuperUpdate program version.
        /// </summary>
        public Version SuperUpdateVersion
        {
            get
            {
                return new Version(Application.ProductVersion);
            }
        }
        /// <summary>
        /// This method re-launches the program with the SuperUpdateArguments.
        /// </summary>
        public void ReLaunch()
        {
            Misc.ReLaunch();
        }
    }
}
