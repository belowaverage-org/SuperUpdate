using System;
using System.Windows.Forms;
using SuperUpdate.Engines;
using SuperUpdate.Log;

namespace SuperUpdate
{
    /// <summary>
    /// The main Program class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Arguments = args;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += Application_ThreadException;
            MainForm = new Main();
            Application.Run(MainForm);
        }
        private static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            Logger.Log("A FATAL UNCAUGHT ERROR HAS OCCURED", e.Exception);
        }
        /// <summary>
        /// The arguments passed to Super Update at launch.
        /// </summary>
        public static string[] Arguments;
        /// <summary>
        /// The running instance of the MainForm class.
        /// </summary>
        public static Main MainForm = null;
        /// <summary>
        /// The running instance of the InstallEngine class.
        /// </summary>
        public static InstallEngine InstallEngine = null;
        /// <summary>
        /// The running instance of the UpdateEngine class.
        /// </summary>
        public static UpdateEngine UpdateEngine = null;
        /// <summary>
        /// The running instance of the UpdateSelectEngine class.
        /// </summary>
        public static UpdateSelectEngine UpdateSelectEngine = null;
        /// <summary>
        /// The running instance of the XmlEngine class.
        /// </summary>
        public static XmlEngine XmlEngine = null;
    }
}
