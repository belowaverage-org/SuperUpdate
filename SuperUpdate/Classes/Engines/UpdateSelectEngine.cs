using System;
using System.Windows.Forms;
using System.Xml;
using SuperUpdate.Log;
using System.Diagnostics;

namespace SuperUpdate.Engines
{
    /// <summary>
    /// This class is used to display a list of updates on the selected list view.
    /// </summary>
    public class UpdateSelectEngine
    {
        private readonly ListView ListView = null;
        /// <summary>
        /// This constructor will display a list of updates on the provided ListView.
        /// </summary>
        /// <param name="ListView">ListView: The list view to display the updates on.</param>
        public UpdateSelectEngine(ListView ListView)
        {
            this.ListView = ListView;
            ListView.SuspendLayout();
            ListView.BeginUpdate();
            Logger.AllowDraw();
            Logger.DrawNewLogs();
            Logger.DrawEnabled = false;
            ListView.Groups.Clear();
            ListView.Columns.Clear();
            ListView.Items.Clear();
            ListView.Columns.Add("Version");
            ListView.Columns.Add("Release Date");
            ListView.Columns.Add("Release Notes");
            ListView.DoubleClick += ListView_DoubleClick;
            foreach (string channel in UpdateEngine.AvailableChannels)
            {
                ListView.Groups.Add(new ListViewGroup(channel, channel));
            }
            foreach (XmlNode update in XmlEngine.UpdateXML.SelectNodes("/SU:SuperUpdate/SU:Updates/SU:Update", XmlEngine.XNS))
            {
                XmlAttributeCollection updateAttribs = update.Attributes;
                if (update.Attributes["ScriptURL"] == null) continue;
                ListViewItem lvItem = ListView.Items.Add(updateAttribs["Version"].Value);
                if (update == UpdateEngine.LatestVersion) lvItem.Selected = true;
                lvItem.Tag = update;
                lvItem.Group = ListView.Groups[update.Attributes["Channel"].Value];
                if (updateAttribs["DateTime"] != null) lvItem.SubItems.Add(DateTime.Parse(updateAttribs["DateTime"].Value).ToString());
                else lvItem.SubItems.Add("");
                if (updateAttribs["ReleaseInfoURL"] != null) lvItem.SubItems.Add(updateAttribs["ReleaseInfoURL"].Value);
                else lvItem.SubItems.Add("");
            }
            ListView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            ListView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            ListView.EndUpdate();
            ListView.ResumeLayout();
        }
        /// <summary>
        /// This method will dispose all resources and event handlers associated with displaying the updates using the existing list view on the main window.
        /// </summary>
        public void Dispose()
        {
            ListView.DoubleClick -= ListView_DoubleClick;
            ListView.Groups.Clear();
            Logger.DrawEnabled = true;
            Logger.AllowDraw();
            Logger.DrawAllLogs();
            GC.SuppressFinalize(this);
        }
        private void ListView_DoubleClick(object sender, EventArgs e)
        {
            ListViewItem lvItem = ListView.SelectedItems[0];
            XmlAttribute releaseInfo = ((XmlNode)lvItem.Tag).Attributes["ReleaseInfoURL"];
            if (releaseInfo == null) return;
            Process.Start(releaseInfo.Value);
        }
    }
}
