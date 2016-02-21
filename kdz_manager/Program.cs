﻿/// @mainpage
/// Hello wrold
///
///
/// TODO: Test OpenFileDialog try to click open when multiple files are selected.
/// TODO: conditionall add final semicolon to user supplied cutsom query. (in user filter)
/// TODO: Run bugtests on adding recent files. Make sure lambda expressions do not cahce old filenames.
///
///
///
///

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
            Application.Run(new MainForm());
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
