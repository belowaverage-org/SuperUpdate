using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SuperUpdate.Log
{
    class Logger
    {
        public static int MaxLogsToDraw = 100;
        public static void Log(string Message, LogLevels LogLevel = LogLevels.Verbose)
        {
            LogItems.Add(new LogItem(Message, LogLevel));
            Refresh();
        }
        public static void Log(string Message, Exception Exception)
        {
            LogItems.Add(new LogItem(Message, LogLevels.Exception));
            Log(Exception);
        }
        public static void Log(Exception Exception)
        {
            LogItems.Add(new LogItem(Exception.Message, LogLevels.Exception));
            foreach (string line in Exception.StackTrace.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
            {
                LogItems.Add(new LogItem(line, LogLevels.Exception));
            }
            if(Exception.InnerException != null)
            {
                Log(Exception.InnerException);
            }
            else
            {
                Refresh();
            }
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
                        logItem.Level.ToString() + " : " +
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
                if (x.Level == LogLevels.Information) return true;
                return false;
            }));
            if (LastInfo != null) Program.MainForm.lblMessage.Text = LastInfo.Message;
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
                ListViewItem item = lv.Items.Add("");
                if (
                    logItem.Level == LogLevels.Verbose ||
                    logItem.Level == LogLevels.Information
                ) item.ImageKey = "info";
                if (logItem.Level == LogLevels.Warning) item.ImageKey = "warn";
                if (logItem.Level == LogLevels.Exception) item.ImageKey = "error";
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
        public LogLevels Level = LogLevels.Information;
        public string Message = "";
        public LogItem(string Message, LogLevels Severity = LogLevels.Information)
        {
            this.Message = Message;
            Level = Severity;
        }
    }
    public enum LogLevels
    {
        Information,
        Warning,
        Exception,
        Verbose
    }
}