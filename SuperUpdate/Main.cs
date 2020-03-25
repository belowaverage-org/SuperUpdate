using System;
using System.ComponentModel;
using System.Windows.Forms;
using SuperUpdate.Log;
using SuperUpdate.Xml;
using SuperUpdate.Update;
using System.Drawing;

namespace SuperUpdate
{
    public partial class Main : Form
    {
        private bool IsRunning = false;
        private bool IsExpanded = false;
        private bool IsMouseOverArrow = false;
        public Main()
        {
            InitializeComponent();
            Icon = Properties.Resources.supersuite;
        }
        private bool Running
        {
            get { return IsRunning; }
            set
            {
                IsRunning = value;
                btnAction.Enabled = !value;
            }
        }
        private bool Expanded
        {
            get { return IsExpanded; }
            set
            {
                IsExpanded = value;
                if (IsExpanded)
                {
                    lvDetails.Show();
                    lblMoreLessInfo.Text = "&Less information";
                }
                else
                {
                    lvDetails.Hide();
                    lblMoreLessInfo.Text = "&More information";
                }
                pbArrow.Invalidate();
            }
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
            Running = true;
            Logger.Initialize();
            Logger.Log("Super Update: v" + ProductVersion.ToString());
            Logger.Log("Developed by: Dylan Bickerstaff (C) 2020");
            Logger.Log("Starting Super Update...", LogLevels.Information);
            if (Program.Arguments.Length == 1)
            {
                CheckForUpdates();
            }
            else
            {
                Logger.Log("XML path has not been passed to Super Update!", LogLevels.Warning);
            }
            Running = Expanded = false;
            CheckIfExpanded();
        }
        private async void CheckForUpdates()
        {
            if (!await XmlEngine.ReadXML(Program.Arguments[0])) return;
            if (!await UpdateEngine.DetectCurrentVersion()) return;
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
            if (e.Button == MouseButtons.Right)
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
        private void btnAction_Click(object sender, EventArgs e)
        {
            Running = true;
            CheckForUpdates();
            Running = false;
        }
        private void ExpandContract(object sender, EventArgs e)
        {
            if (Expanded)
            {
                Size = MinimumSize;
            }
            else
            {
                Size = new Size(800, 500);
            }
            CenterToScreen();
        }
        private void CheckIfExpanded(object sender = null, EventArgs e = null)
        {
            if(Expanded != (Size.Height != MinimumSize.Height))
            {
                Expanded = Size.Height != MinimumSize.Height;
            }
        }
        private void pbArrow_MouseEnter(object sender, EventArgs e)
        {
            IsMouseOverArrow = true;
            pbArrow.Invalidate();
        }
        private void pbArrow_MouseLeave(object sender, EventArgs e)
        {
            IsMouseOverArrow = false;
            pbArrow.Invalidate();
        }
        private void pbArrow_Paint(object sender, PaintEventArgs e)
        {
            Image arrow = new Bitmap(Properties.Resources.downarrow);
            if (Expanded) arrow.RotateFlip(RotateFlipType.RotateNoneFlipY);
            Graphics g = e.Graphics;
            g.Clear(Color.WhiteSmoke);
            g.DrawImage(arrow, 0, 0, pbArrow.Width, pbArrow.Height);
            if (!IsMouseOverArrow)
            {
                g.FillRectangle(new SolidBrush(Color.FromArgb(50, Color.WhiteSmoke)), 0, 0, pbArrow.Width, pbArrow.Height);
            }
            arrow.Dispose();
        }
    }
}