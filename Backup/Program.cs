using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Common;

namespace MessageViewer
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            if (Config.Load()== "OK")
            {
                Application.EnableVisualStyles();
                Application.Run(new TransViewer());
            }
            else
                MessageBox.Show(Config.Status);
        }
    }
}