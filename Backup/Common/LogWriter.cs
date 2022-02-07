using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace Common
{
    class LogWriter
    {
        #region Fields...
        long lngLogID;
        string strCurrentFileLog;
        object sync;
        #endregion

        #region Constructors...
        public LogWriter()
        {
            lngLogID = 0;

            sync = new object();
            CheckLogFolders();
        }
        #endregion

        #region Porperties...
        #endregion

        #region Private Methods...
        private void CheckLogFolders()
        {
            DirectoryInfo fiApplicationPath;

            //Assign App Path to the Directory Info Object
            fiApplicationPath = new DirectoryInfo(Application.StartupPath);

            //Check if Logs Folder exists
            if (! Directory.Exists(Application.StartupPath + @"\Logs"))
                fiApplicationPath.CreateSubdirectory("Logs");

        }
        #endregion

        #region Exposed Methods...
        public void Append(string strTransDescription, string strModule)
        {
            Append(0, 0, 0, 0, strTransDescription, strModule);
        }

        public void Append(long lngSocketID, long lngSocketTransID, long intSpliterTransID,
            long lngThreadContTransID, string strTransDescription, string strModule)
        {
            DateTime now = DateTime.Now;
            string timestamp = now.ToString("yy-MM-dd hh:mm:ss:fff");

            lock (sync)
            {
                //Get New Log ID
                lngLogID++;

                //Get File Name
                string strModuleFolder = @"\Logs";
                strCurrentFileLog = Application.StartupPath + strModuleFolder + @"\Log_" + now.ToString("yyyyMMdd") + ".txt";

                //Build log line
                StringBuilder strLogLine = new StringBuilder();
                strLogLine.
                AppendLine(strModule).Append("\t").
                Append(lngSocketID).Append("\t").
                Append(lngSocketTransID).Append("\t").
                Append(intSpliterTransID).Append("\t").
                Append(lngThreadContTransID).Append("\t").
                Append(lngLogID).Append("\t").
                Append(timestamp).Append("\t").
                Append(strTransDescription);
                try
                {
                    //Open File
                    FileStream fsSave = new FileStream(strCurrentFileLog, FileMode.Append, FileAccess.Write, FileShare.Write);
                    StreamWriter srWriter = new StreamWriter(fsSave);

                    //Write into Log
                    srWriter.WriteLine(strLogLine);
                    srWriter.Close();
                    fsSave.Close();
                }
                catch { }

                Thread.Sleep(5);

            }
        }
        #endregion
    }
}
