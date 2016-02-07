using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace kdz_manager
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainUnhandledException;
            Application.ThreadException += ApplicationThreadException;
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
        static void CurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            MessageBox.Show("hello");
            // Write to log file.
            //throw new NotImplementedException();
        }

        static void ApplicationThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            MessageBox.Show("hello");
            // Write to log file.
            //throw new NotImplementedException();
        }
    }
}
