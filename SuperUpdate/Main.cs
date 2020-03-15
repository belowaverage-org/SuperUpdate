using System;
using System.ComponentModel;
using System.Windows.Forms;
using SuperUpdate.Log;
using SuperUpdate.Xml;

namespace SuperUpdate
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
            Icon = Properties.Resources.supersuite;
        }
        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            wbAnimation.Document.Body.Style = "margin:0px;";
            string gif = Convert.ToBase64String(Properties.Resources.spinner);
            gif = "<img ondragstart=\"return false;\" style=\"width:100%;height:100%;top:0px;left:0px;position:fixed;\" src=\"data:image/gif;base64," + gif + "\" />";
            wbAnimation.Document.Body.InnerHtml = gif;
        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
        private void Main_HelpButtonClicked(object sender, CancelEventArgs e)
        {
            new About().ShowDialog();
            e.Cancel = true;
        }
        private void Main_Load(object sender, EventArgs e)
        {
            Logger.Initialize();
            Logger.Log("Super Update: v" + ProductVersion.ToString());
            Logger.Log("Developed by: Dylan Bickerstaff (C) 2020");
            Logger.Log("Starting Super Update...", LogLevels.Information);
            if (Program.Arguments.Length == 1)
            {
                XmlEngine.ReadRemoteXML(Program.Arguments[0]);
            }
            else
            {
                Logger.Log("XML URL has not been passed to Super Update!", LogLevels.Warning);
            }

            Timer timer = new Timer();
            timer.Interval = 1;
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            Logger.Log("Hi");
        }

        private async void miSaveLog_Click(object sender, EventArgs e)
        {
            diSaveLog.FileName =
            "SuperUpdate_" +
            DateTime.Now.ToShortDateString().Replace('/', '-') + "_" +
            DateTime.Now.ToLongTimeString().Replace(':', '-');
            if (diSaveLog.ShowDialog() == DialogResult.OK)
            {
                await Logger.WriteLog(diSaveLog.OpenFile());
            }
        }
        private void lvDetails_MouseClick(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Right)
            {
                miLog.Show(lvDetails, e.Location);
            }
        }

        private void Main_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.S)
            {
                miSaveLog.PerformClick();
            }
        }
    }
}