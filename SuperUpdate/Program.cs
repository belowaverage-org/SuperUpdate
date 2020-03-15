using System;
using System.Windows.Forms;

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
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            MainForm = new Main();
            Application.Run(MainForm);
        }
        public static Main MainForm = null;
    }
}
