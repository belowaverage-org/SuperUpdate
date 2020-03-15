using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SuperUpdate
{
    class Logger
    {
        public static int MaxLogsToDraw = 100;
        public static void Log(string Message, LogItem.LogSeverity Severity = LogItem.LogSeverity.Information)
        {
            LogItems.Add(new LogItem(Message, Severity));
            if (Severity == LogItem.LogSeverity.Verbose) return;
            Refresh();
        }
        public static void Refresh()
        {
            if (Program.MainForm.InvokeRequired)
            {
                Program.MainForm.Invoke(DrawLogs);
            }
            else
            {
                DrawLogs.DynamicInvoke();
            }
        }
        public static Task WriteLog(Stream Stream)
        {
            Log("Writing log to disk...");
            return Task.Run(() => {
                StreamWriter writer = new StreamWriter(Stream);
                foreach (LogItem logItem in LogItems)
                {
                    writer.WriteLine(
                        logItem.Severity.ToString() + " : " +
                        logItem.TimeStamp.ToString() + " > " +
                        logItem.Message
                    );
                }
                writer.Flush();
                writer.Close();
                Log("Done.");
            });
        }
        public static void Initialize()
        {
            ImageList.ImageSize = new System.Drawing.Size(16, 16);
            ImageList.Images.Add("info", Properties.Resources.info);
            ImageList.Images.Add("warn", Properties.Resources.warn);
            ImageList.Images.Add("error", Properties.Resources.error);
        }
        private static Delegate DrawLogs = new Action(() => {
            LogItem LastInfo = LogItems.FindLast(new Predicate<LogItem>((x) => {
                if (x.Severity == LogItem.LogSeverity.Information) return true;
                return false;
            }));
            Program.MainForm.lblMessage.Text = LastInfo.Message;
            ListView lv = Program.MainForm.lvDetails;
            lv.BeginUpdate();
            lv.SuspendLayout();
            lv.SmallImageList = ImageList;
            lv.Columns.Clear();
            lv.Columns.Add("");
            lv.Columns.Add("Time");
            lv.Columns.Add("Description");
            lv.Items.Clear();
            List<LogItem> logItems = null;
            if(LogItems.Count >= MaxLogsToDraw)
            {
                logItems = LogItems.GetRange(LogItems.Count - MaxLogsToDraw, MaxLogsToDraw);
            }
            else
            {
                logItems = LogItems;
            }
            foreach(LogItem logItem in logItems)
            {
                if (logItem.Severity == LogItem.LogSeverity.Verbose) continue;
                ListViewItem item = lv.Items.Add("");
                if (logItem.Severity == LogItem.LogSeverity.Information) item.ImageKey = "info";
                if (logItem.Severity == LogItem.LogSeverity.Warning) item.ImageKey = "warn";
                if (logItem.Severity == LogItem.LogSeverity.Exception) item.ImageKey = "error";
                item.SubItems.Add(logItem.TimeStamp.ToLongTimeString());
                item.SubItems.Add(logItem.Message);
            }
            lv.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            lv.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            lv.EnsureVisible(lv.Items.Count - 1);
            lv.ResumeLayout();
            lv.EndUpdate();
        });
        private static List<LogItem> LogItems = new List<LogItem>();
        private static ImageList ImageList = new ImageList();
    }
    class LogItem
    {
        public DateTime TimeStamp = DateTime.Now;
        public LogSeverity Severity = LogSeverity.Information;
        public string Message = "";
        public LogItem(string Message, LogSeverity Severity = LogSeverity.Information)
        {
            this.Message = Message;
            this.Severity = Severity;
        }
        public enum LogSeverity
        {
            Information,
            Warning,
            Exception,
            Verbose
        }
    }
}