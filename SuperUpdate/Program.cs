using System;
using System.Windows.Forms;
using SuperUpdate.Log;

namespace SuperUpdate
{
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
        public static Main MainForm = null;
        public static string[] Arguments;
    }
}
