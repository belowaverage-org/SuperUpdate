using System;
using System.ComponentModel;
using System.Windows.Forms;
using SuperUpdate.Log;
using SuperUpdate.Engines;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using System.Diagnostics;
using System.Collections.Generic;

namespace SuperUpdate
{
    public partial class Main : Form
    {
        public Size ExpandedSize = new Size(800, 500);
        public string LargeImageStatic = "";
        public string LargeImageSpinner = "";
        public bool AutoRun = false;
        private bool IsRunning = false;
        private bool IsExpanded = false;
        private bool IsMouseOverArrow = false;
        
        public Main()
        {
            InitializeComponent();
            Expanded = false;
            Size = MinimumSize;
            Icon = Properties.Resources.logo;
        }
        private bool Running
        {
            get { return IsRunning; }
            set
            {
                IsRunning = value;
                if (IsRunning)
                {
                    pbMain.Style = ProgressBarStyle.Marquee;
                }
                else
                {
                    pbMain.Style = ProgressBarStyle.Continuous;
                    pbMain.Value = 0;
                }
                RefreshLargeIcon();
            }
        }
        public bool Expanded
        {
            get { return IsExpanded; }
            set
            {
                IsExpanded = value;
                if (IsExpanded)
                {
                    lvDetails.Show();
                    lblMoreLessInfo.Text = "&Fewer details";
                }
                else
                {
                    lvDetails.Hide();
                    lblMoreLessInfo.Text = "&More details";
                }
                pbArrow.Invalidate();
            }
        }
        public void RefreshLargeIcon(object sender = null, WebBrowserDocumentCompletedEventArgs e = null)
        {
            string LargeImage = "";
            if (wbAnimation.Document.Body == null) return;
            if (!Running) LargeImage = LargeImageStatic;
            if (Running) LargeImage = LargeImageSpinner;
            wbAnimation.Document.Body.Style = "margin:0px;";
            wbAnimation.Document.Body.InnerHtml = "<img ondragstart=\"return false;\" style=\"width:100%;height:100%;top:0px;left:0px;position:fixed;\" src=\"" + LargeImage + "\" />";
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
        private async void Main_Load(object sender = null, EventArgs e = null)
        {
            Activate();
            bool success = true;
            Running = true;
            await GetImagesFromResources();
            Logger.Initialize();
            Logger.Log("Super Update: v" + ProductVersion.ToString());
            Logger.Log("Developed by: Dylan Bickerstaff (C) 2020");
            Logger.Log("Starting Super Update...", LogLevels.Information);
            success = await CheckForUpdates();
            if (!success)
            {
                Logger.Log("Something went wrong, press \"More details\" for more details.", LogLevels.Information);
                Running = false;
                return;
            }
            Program.UpdateSelectEngine = new UpdateSelectEngine(lvDetails);
            Running = AutoRun;
            lvDetails_SelectedIndexChanged(null, null);
            if (AutoRun) btnAction_Click(null, null);
        }
        private async Task<bool> CheckForUpdates()
        {
            bool success = true;
            string xmlUrl = "";
            FileStream fs = File.OpenRead(Process.GetCurrentProcess().MainModule.FileName);
            StreamReader sr = new StreamReader(fs);
            fs.Seek(-2048, SeekOrigin.End);
            List<string> srLines = new List<string>();
            while (!sr.EndOfStream) srLines.Add(await sr.ReadLineAsync());
            if (
                srLines.Count >= 2 &&
                srLines[srLines.Count - 2] == "" &&
                srLines[srLines.Count - 1] != "" &&
                Uri.TryCreate(srLines[srLines.Count - 1], UriKind.RelativeOrAbsolute, out Uri uri)
            )
            {
                xmlUrl = uri.ToString();
            }
            if (Program.Arguments.Length == 1)
            {
                Logger.Log("XML passed via CLI.");
                xmlUrl = Program.Arguments[0];
            } 
            else if (xmlUrl != "")
            {
                Logger.Log("XML passed via binary.");
            }
            else
            {
                Logger.Log("XML path has not been passed to Super Update!", LogLevels.Exception);
                return false;
            }
            success = await XmlEngine.ReadXML(xmlUrl);
            if (!success) return success;
            success = await UpdateEngine.DetectUpdates();
            if (!success) return success;
            if (UpdateEngine.CurrentVersion != UpdateEngine.LatestVersion)
            {
                if (UpdateEngine.LatestVersion.Attributes["UpdateMessage"] != null)
                {
                    Logger.Log(UpdateEngine.LatestVersion.Attributes["UpdateMessage"].Value, LogLevels.Information);
                }
                else
                {
                    Logger.Log("Found new version, press \"Install\" to begin.", LogLevels.Information);
                }
            }
            else
            {
                XmlNode noUpdateMessage = XmlEngine.UpdateXML.SelectSingleNode("/SU:SuperUpdate/SU:Settings/SU:MessageNoUpdate", XmlEngine.XNS);
                if (noUpdateMessage != null)
                {
                    Logger.Log(noUpdateMessage.Attributes["Text"].Value, LogLevels.Information);
                }
                else
                {
                    Logger.Log("No updates are available.", LogLevels.Information);
                }
            }
            return success;
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
        private async void btnAction_Click(object sender, EventArgs e)
        {
            if (lvDetails.SelectedItems.Count == 0) return;
            Running = true;
            Program.InstallEngine = new InstallEngine();
            Task<bool> install = Program.InstallEngine.InstallUpdate((XmlNode)lvDetails.SelectedItems[0].Tag);
            if (Program.UpdateSelectEngine != null)
            {
                Program.UpdateSelectEngine.Dispose();
                Program.UpdateSelectEngine = null;
            }
            if (!await install) Logger.Log("Something went wrong, press \"More details\" for more details.", LogLevels.Information);
            Running = false;
            btnAction.Enabled = false;
            btnCancel.Text = "Close";
        }
        public void ExpandContract(object sender = null, EventArgs e = null)
        {
            if (Expanded)
            {
                Size = MinimumSize;
            }
            else
            {
                Size = ExpandedSize;
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
        private Task GetImagesFromResources()
        {
            return Task.Run(() => {
                MemoryStream stream = new MemoryStream();
                Bitmap btm = new Bitmap(64, 64);
                Graphics g = Graphics.FromImage(btm);
                g.DrawIcon(new Icon(Properties.Resources.logo, 48, 48), 8, 8);
                btm.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                g.Dispose();
                string img = Convert.ToBase64String(stream.GetBuffer());
                stream.Dispose();
                LargeImageStatic = "data:image/png;base64," + img;
                string gif = Convert.ToBase64String(Properties.Resources.spinner);
                LargeImageSpinner = "data:image/gif;base64," + gif;
            });
        }
        public void CenterWindow()
        {
            CenterToScreen();
        }
        private void lvDetails_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (
                Program.UpdateSelectEngine != null &&
                lvDetails.SelectedItems.Count == 1 &&
                !Running
            ) btnAction.Enabled = true;
            else btnAction.Enabled = false;
        }
        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Program.InstallEngine != null) Program.InstallEngine.Stop();
            if (Running)
            {
                e.Cancel = true;
            }
        }
    }
}