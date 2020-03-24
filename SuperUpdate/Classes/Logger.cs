using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace SuperUpdate.Log
{
    class Logger
    {
        public static int MaxLogsToDraw = 100;
        public static void Log(string Message, LogLevels LogLevel = LogLevels.Verbose)
        {
            LogLock.EnterWriteLock();
            LogItems.Add(new LogItem(Message, LogLevel));
            LogLock.ExitWriteLock();
            DrawNewLogs();
        }
        public static void Log(string Message, Exception Exception)
        {
            LogLock.EnterWriteLock();
            LogItems.Add(new LogItem(Message, LogLevels.Exception));
            LogLock.ExitWriteLock();
            Log(Exception);
        }
        public static void Log(Exception Exception)
        {
            LogLock.EnterWriteLock();
            LogItems.Add(new LogItem(Exception.Message, LogLevels.Exception));
            if (Exception.StackTrace != null)
            {
                foreach (string line in Exception.StackTrace.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    LogItems.Add(new LogItem(line, LogLevels.Exception));
                }
            }
            LogLock.ExitWriteLock();
            if (Exception.InnerException != null)
            {
                Log(Exception.InnerException);
            }
            else
            {
                DrawNewLogs();
            }
        }
        public static void DrawNewLogs()
        {
            if (!CanDrawNewLogs)
            {
                DrawQueued = true;
                return;
            }
            if (Program.MainForm.InvokeRequired)
            {
                Program.MainForm.Invoke(DrawLogs);
            }
            else
            {
                DrawLogs.DynamicInvoke();
            }
            CanDrawNewLogs = DrawQueued = false;
        }
        public static Task WriteLog(Stream Stream)
        {
            Log("Writing log to disk...");
            return Task.Run(() => {
                StreamWriter writer = new StreamWriter(Stream);
                LogLock.EnterReadLock();
                foreach (LogItem logItem in LogItems)
                {
                    writer.WriteLine(
                        logItem.Level.ToString() + " : " +
                        logItem.TimeStamp.ToString() + " > " +
                        logItem.Message
                    );
                }
                LogLock.ExitReadLock();
                writer.Flush();
                writer.Close();
                Log("Done.");
            });
        }
        public static void Initialize()
        {
            RefreshTimer.Interval = 100;
            RefreshTimer.Start();
            RefreshTimer.Tick += RefreshTimer_Tick;
            ImageList.ImageSize = new System.Drawing.Size(16, 16);
            ImageList.Images.Add("info", Properties.Resources.info);
            ImageList.Images.Add("warn", Properties.Resources.warn);
            ImageList.Images.Add("error", Properties.Resources.error);
            ClearAllLogs();
        }
        public static void DrawAllLogs()
        {
            ClearAllLogs();
            LogLock.EnterWriteLock();
            foreach(LogItem log in LogItems) log.Drawn = false;
            LogLock.ExitWriteLock();
            DrawNewLogs();
        }
        private static void ClearAllLogs()
        {
            ListView lv = Program.MainForm.lvDetails;
            lv.SmallImageList = ImageList;
            lv.Columns.Clear();
            lv.Columns.Add("");
            lv.Columns.Add("Time");
            lv.Columns.Add("Description");
            lv.Items.Clear();
        }
        private static void RefreshTimer_Tick(object sender, EventArgs e)
        {
            CanDrawNewLogs = true;
            if (DrawQueued) DrawNewLogs();
        }
        private static Delegate DrawLogs = new Action(() => {
            LogLock.EnterReadLock();
            LogItem LastInfo = LogItems.FindLast(new Predicate<LogItem>((x) => {
                if (x.Level == LogLevels.Information) return true;
                return false;
            }));
            LogLock.ExitReadLock();
            if (LastInfo != null) Program.MainForm.lblMessage.Text = LastInfo.Message;
            ListView lv = Program.MainForm.lvDetails;
            lv.BeginUpdate();
            lv.SuspendLayout();
            List<ListViewItem> newListItems = new List<ListViewItem>();
            LogLock.EnterWriteLock();
            List<LogItem> logsToBeDrawn = LogItems.FindAll(new Predicate<LogItem>((x) => {
                return !x.Drawn;
            }));
            foreach (LogItem logItem in logsToBeDrawn)
            {
                ListViewItem item = new ListViewItem("");
                if (
                    logItem.Level == LogLevels.Verbose ||
                    logItem.Level == LogLevels.Information
                ) item.ImageKey = "info";
                if (logItem.Level == LogLevels.Warning) item.ImageKey = "warn";
                if (logItem.Level == LogLevels.Exception) item.ImageKey = "error";
                item.SubItems.Add(logItem.TimeStamp.ToLongTimeString());
                item.SubItems.Add(logItem.Message);
                logItem.Drawn = true;
                newListItems.Add(item);
            }
            LogLock.ExitWriteLock();
            lv.Items.AddRange(newListItems.ToArray());
            while(lv.Items.Count > MaxLogsToDraw)
            {
                lv.Items.RemoveAt(0);
            }
            lv.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            lv.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            lv.EnsureVisible(lv.Items.Count - 1);
            lv.ResumeLayout();
            lv.EndUpdate();
        });
        private static bool CanDrawNewLogs = true;
        private static bool DrawQueued = true;
        private static ReaderWriterLockSlim LogLock = new ReaderWriterLockSlim();
        private static System.Windows.Forms.Timer RefreshTimer = new System.Windows.Forms.Timer();
        private static List<LogItem> LogItems = new List<LogItem>();
        private static ImageList ImageList = new ImageList();
    }
    class LogItem
    {
        public bool Drawn = false;
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