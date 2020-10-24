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
    /// <summary>
    /// The main window class.
    /// </summary>
    public partial class Main : Form
    {
        /// <summary>
        /// The size of the main window when it is in its expanded state.
        /// </summary>
        public Size ExpandedSize = new Size(800, 500);
        /// <summary>
        /// The URI of a large static image to display to a user when the program is not busy.
        /// </summary>
        public string LargeImageStatic = "";
        /// <summary>
        /// The URI of a large image spinner gif to display to a user when the program is busy.
        /// </summary>
        public string LargeImageSpinner = "";
        /// <summary>
        /// If true, once the CheckForUpdates method has ran, the update process will start automatically.
        /// </summary>
        public bool AutoRun = false;
        private bool IsRunning = false;
        private bool IsExpanded = false;
        private bool IsMouseOverArrow = false;
        /// <summary>
        /// The constructor for the main window.
        /// </summary>
        public Main()
        {
            InitializeComponent();
            Expanded = false;
            Size = MinimumSize;
            Icon = Properties.Resources.logo;
        }
        /// <summary>
        /// If true, the program is busy doing a task.
        /// </summary>
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
        /// <summary>
        /// If true, the main window is expanded.
        /// </summary>
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
        /// <summary>
        /// This method will check for updates in the main form and display them to the user.
        /// </summary>
        /// <returns>bool: True on success.</returns>
        private async Task<bool> CheckForUpdates()
        {
            bool success;
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
        /// <summary>
        /// Toggles the window to be either expanded or contracted to the ExpandedSize and MinimumSize.
        /// </summary>
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
        /// <summary>
        /// Loads in LargeImageStatic or LargeImageSpinner when this method is run depending on whether or not IsRunning is true.
        /// </summary>
        public void RefreshLargeIcon(object sender = null, WebBrowserDocumentCompletedEventArgs e = null)
        {
            string LargeImage = "";
            if (wbAnimation.Document.Body == null) return;
            if (!Running) LargeImage = LargeImageStatic;
            if (Running) LargeImage = LargeImageSpinner;
            wbAnimation.Document.Body.Style = "margin:0px;";
            wbAnimation.Document.Body.InnerHtml = "<img ondragstart=\"return false;\" style=\"width:100%;height:100%;top:0px;left:0px;position:fixed;\" src=\"" + LargeImage + "\" />";
        }
        /// <summary>
        /// Centers the Main window.
        /// </summary>
        public void CenterWindow()
        {
            CenterToScreen();
        }
        private void CheckIfExpanded(object sender = null, EventArgs e = null)
        {
            if (Expanded != (Size.Height != MinimumSize.Height))
            {
                Expanded = Size.Height != MinimumSize.Height;
            }
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
        private async void Main_Load(object sender = null, EventArgs e = null)
        {
            Activate();
            bool success;
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
            LV_Details_SelectedIndexChanged(null, null);
            if (AutoRun) BTN_Action_Click(null, null);
        }
        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Program.InstallEngine != null) Program.InstallEngine.Stop();
            if (Running)
            {
                e.Cancel = true;
            }
        }
        private void Main_HelpButtonClicked(object sender, CancelEventArgs e)
        {
            new About().ShowDialog();
            e.Cancel = true;
        }
        private void Main_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.S)
            {
                miSaveLog.PerformClick();
            }
        }
        private async void MI_SaveLog_Click(object sender, EventArgs e)
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
        private void LV_Details_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                miLog.Show(lvDetails, e.Location);
            }
        }
        private void LV_Details_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (
                Program.UpdateSelectEngine != null &&
                lvDetails.SelectedItems.Count == 1 &&
                !Running
            ) btnAction.Enabled = true;
            else btnAction.Enabled = false;
        }
        private async void BTN_Action_Click(object sender, EventArgs e)
        {
            if (lvDetails.SelectedItems.Count == 0) return;
            btnAction.Enabled = false;
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
            btnCancel.Text = "Close";
        }
        private void BTN_Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }
        private void PB_Arrow_MouseEnter(object sender, EventArgs e)
        {
            IsMouseOverArrow = true;
            pbArrow.Invalidate();
        }
        private void PB_Arrow_MouseLeave(object sender, EventArgs e)
        {
            IsMouseOverArrow = false;
            pbArrow.Invalidate();
        }
        private void PB_Arrow_Paint(object sender, PaintEventArgs e)
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