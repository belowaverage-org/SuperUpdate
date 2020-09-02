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
        public string[] Arguments
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
        public XmlNode CurrentVersion
        {
            get
            {
                return UpdateEngine.CurrentVersion;
            }
        }
        public XmlNode LatestVersion
        {
            get
            {
                return UpdateEngine.LatestVersion;
            }
        }
        public List<LogItem> LogItems
        {
            get
            {
                return Logger.LogItems;
            }
        }
        public void ReLaunch()
        {
            Misc.ReLaunch();
        }
    }
}
