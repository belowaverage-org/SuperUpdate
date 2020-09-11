using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace SuperUpdate.Log
{
    /// <summary>
    /// This class represents the logging system.
    /// </summary>
    public static class Logger
    {
        /// <summary>
        /// A list of all logs since the program started.
        /// </summary>
        public static List<LogItem> LogItems = new List<LogItem>();
        /// <summary>
        /// This integer represents the maximum amount of logs to display on the list view.
        /// </summary>
        public static int MaxLogsToDraw = 100;
        /// <summary>
        /// This boolean determines whether or not to draw logs to the list view.
        /// </summary>
        public static bool DrawEnabled = true;
        /// <summary>
        /// This method will write a new log entry.
        /// </summary>
        /// <param name="Message">String: The main message of the log.</param>
        /// <param name="LogLevel">LogLevels: The severity level of the log.</param>
        public static void Log(string Message, LogLevels LogLevel = LogLevels.Verbose)
        {
            LogLock.EnterWriteLock();
            LogItems.Add(new LogItem(Message, LogLevel));
            LogLock.ExitWriteLock();
            DrawNewLogs();
        }
        /// <summary>
        /// This method will write an exception with a message to the log.
        /// </summary>
        /// <param name="Message">String: Message to include with the exception.</param>
        /// <param name="Exception">Exception: The exception to include in the log.</param>
        public static void Log(string Message, Exception Exception)
        {
            LogLock.EnterWriteLock();
            LogItems.Add(new LogItem(Message, LogLevels.Exception));
            LogLock.ExitWriteLock();
            Log(Exception);
        }
        /// <summary>
        /// This method will write an exception to the log.
        /// </summary>
        /// <param name="Exception">Exception: The exception to write to the log.</param>
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
        /// <summary>
        /// This method will draw only new logs to the list view.
        /// </summary>
        public static void DrawNewLogs()
        {
            if (!DrawEnabled) return;
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
        /// <summary>
        /// This method will write all the logs to a stream.
        /// </summary>
        /// <param name="Stream">Stream: The stream to write all logs into.</param>
        /// <returns>Task: Returns an awaiter.</returns>
        public static Task WriteLog(Stream Stream)
        {
            Log("Writing log to disk...");
            return Task.Run(() => {
                StreamWriter writer = new StreamWriter(Stream);
                LogLock.EnterReadLock();
                foreach (LogItem logItem in LogItems)
                {
                    string msg = logItem.Message.Replace("\n", ": ");
                    writer.WriteLine(
                        logItem.Level.ToString() + " : " +
                        logItem.TimeStamp.ToString() + " > " +
                        msg
                    );
                }
                LogLock.ExitReadLock();
                writer.Flush();
                writer.Close();
                Log("Done.");
            });
        }
        /// <summary>
        /// This method will prepare the list view to be able to host log entries.
        /// </summary>
        public static void Initialize()
        {
            RefreshTimer.Interval = 100;
            RefreshTimer.Start();
            RefreshTimer.Tick += AllowDraw;
            ImageList.ImageSize = new System.Drawing.Size(16, 16);
            ImageList.Images.Add("info", Properties.Resources.info);
            ImageList.Images.Add("warn", Properties.Resources.warn);
            ImageList.Images.Add("error", Properties.Resources.error);
            ClearAllLogs();
        }
        /// <summary>
        /// This method will force re-draw all logs in the main window.
        /// </summary>
        public static void DrawAllLogs()
        {
            ClearAllLogs();
            LogLock.EnterWriteLock();
            foreach(LogItem log in LogItems) log.Drawn = false;
            LogLock.ExitWriteLock();
            DrawNewLogs();
        }
        /// <summary>
        /// The main window will only re-draw all the logs on a set interval, invoking this method will allow the logs to be drawn immediately.
        /// </summary>
        public static void AllowDraw(object sender = null, EventArgs e = null)
        {
            CanDrawNewLogs = true;
            if (DrawQueued) DrawNewLogs();
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
        private static readonly Delegate DrawLogs = new Action(() => {
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
                string msg = logItem.Message.Replace("\n", ": ");
                item.SubItems.Add(msg);
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
        private static readonly ReaderWriterLockSlim LogLock = new ReaderWriterLockSlim();
        private static readonly System.Windows.Forms.Timer RefreshTimer = new System.Windows.Forms.Timer();
        private static readonly ImageList ImageList = new ImageList();
    }
    /// <summary>
    /// This class represents a log entry.
    /// </summary>
    public class LogItem
    {
        /// <summary>
        /// A boolean value that represents whether or not this log has been displayed to the user.
        /// </summary>
        public bool Drawn = false;
        /// <summary>
        /// The time and date this log was generated.
        /// </summary>
        public DateTime TimeStamp = DateTime.Now;
        /// <summary>
        /// The severity level of this log.
        /// </summary>
        public LogLevels Level = LogLevels.Information;
        /// <summary>
        /// The main message of the log.
        /// </summary>
        public string Message = "";
        /// <summary>
        /// This method constructs a new log entry.
        /// Each instance of a log entry stores information about the log including the date, time, LogLevel, and the message.
        /// </summary>
        /// <param name="Message">String: The main message of the log.</param>
        /// <param name="Severity">LogLevels: The severity of this log. The default is "Information".</param>
        public LogItem(string Message, LogLevels Severity = LogLevels.Information)
        {
            this.Message = Message;
            Level = Severity;
        }
    }
    /// <summary>
    /// This enumeration defines how important a log should be.
    /// The behavior of each log changes depending on these parameters.
    /// </summary>
    public enum LogLevels
    {
        /// <summary>
        /// Represents logs that are for informational purposes.
        /// Logs flagged as "Information" will be displayed to the user on the main window.
        /// </summary>
        Information,
        /// <summary>
        /// Represents logs that indicate an error that can be ignored.
        /// Logs flagged as "Warning" will be displayed in the logs with an exclamation triangle icon.
        /// </summary>
        Warning,
        /// <summary>
        /// Represents logs that indicate a large scale error.
        /// Logs flagged as "Exception" will be displayed in the logs with an "X" icon.
        /// </summary>
        Exception,
        /// <summary>
        /// Represents logs that are meant for debug purposes.
        /// Logs flagged as "Verbose" will be displayed as a normal log in the logs screen.
        /// </summary>
        Verbose
    }
}