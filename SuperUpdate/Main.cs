using System;
using System.ComponentModel;
using System.Windows.Forms;
using SuperUpdate.Log;
using SuperUpdate.Xml;
using SuperUpdate.Update;
using System.Drawing;
using System.Xml;
using System.IO;
using System.Threading.Tasks;

namespace SuperUpdate
{
    public partial class Main : Form
    {
        public Size ExpandedSize = new Size(800, 500);
        public string LargeImageStatic = "";
        public string LargeImageSpinner = "";
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
                if (IsRunning)
                {
                    pbMain.Style = ProgressBarStyle.Marquee;
                }
                else
                {
                    pbMain.Style = ProgressBarStyle.Continuous;
                }
                RefreshLargeIcon();
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
        private async void Main_Load(object sender, EventArgs e)
        {
            Running = true;
            await GetImagesFromResources();
            Expanded = false;
            CheckIfExpanded();
            Logger.Initialize();
            Logger.Log("Super Update: v" + ProductVersion.ToString());
            Logger.Log("Developed by: Dylan Bickerstaff (C) 2020");
            Logger.Log("Starting Super Update...", LogLevels.Information);
            if (Program.Arguments.Length == 1)
            {
                await CheckForUpdates();
            }
            else
            {
                Logger.Log("XML path has not been passed to Super Update!", LogLevels.Warning);
            }
            Running = false;
        }
        private async Task CheckForUpdates()
        {
            if (!await XmlEngine.ReadXML(Program.Arguments[0])) return;
            if (!await UpdateEngine.DetectUpdates()) return;
            
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
            Running = true;
            await CheckForUpdates();
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
                g.DrawIcon(new Icon(Properties.Resources.supersuite, 48, 48), 8, 8);
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
        //public void 
    }
}