using System;
using System.Windows.Forms;

namespace SuperUpdate.PSRunspace
{
    public class PSRunspaceEngine // THIS CLASS NEEDS ACCESS TO THE UPDATE ENGINE CLASS
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
        public string Text
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
        public bool Expanded
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
        public void Close()
        {
            Program.MainForm.Invoke(new Action(() => {
                Program.MainForm.Close();
            }));
        }
    }
}
