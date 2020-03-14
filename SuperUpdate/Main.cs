using System;
using System.ComponentModel;
using System.Windows.Forms;

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
    }
}
