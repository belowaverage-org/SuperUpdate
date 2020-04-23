﻿using System;
using System.Windows.Forms;
using System.Xml;
using SuperUpdate.Log;
using SuperUpdate.Xml;
using System.Diagnostics;

namespace SuperUpdate.Update
{
    class UpdateSelectEngine
    {
        private ListView ListView = null;
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
            ListView.DoubleClick += ListView_DoubleClick;

            foreach (string channel in UpdateEngine.AvailableChannels)
            {
                ListView.Groups.Add(new ListViewGroup(channel, channel));
            }

            foreach (XmlNode update in XmlEngine.UpdateXML.SelectNodes("/SuperUpdate/Updates/Update"))
            {
                XmlAttributeCollection updateAttribs = update.Attributes;
                if (update.Attributes["ScriptURL"] == null) continue;
                ListViewItem lvItem = ListView.Items.Add(updateAttribs["Version"].Value);
                lvItem.Tag = updateAttribs;
                lvItem.Group = ListView.Groups[update.Attributes["Channel"].Value];
                if (updateAttribs["DateTime"] != null)
                {
                    lvItem.SubItems.Add(DateTime.Parse(updateAttribs["DateTime"].Value).ToString());
                }
                else
                {
                    lvItem.SubItems.Add("");
                }
            }
            ListView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            ListView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            ListView.EndUpdate();
            ListView.ResumeLayout();
        }
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
            XmlAttribute releaseInfo = ((XmlAttributeCollection)lvItem.Tag)["ReleaseInfoURL"];
            if (releaseInfo == null) return;
            Process.Start(releaseInfo.Value);
        }
    }
}