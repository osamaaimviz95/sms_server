using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using Janus.Windows.GridEX;

using Common;
using RequestListener;

namespace MessageViewer
{
    public partial class TransViewer : Form
    {
        #region Fields...
        private DataTable lstData = new DataTable();
        private int lngTransColumnPadding = 25;
        private long lngMaxRecordsQuantity = Int32.Parse(Config.Value("Console.MaxRows"));
        private long lngRequestedQty;
        private long lngRequestedPendingQty;
        private long lngProcessedOk;
        private long lngProcessedFail;
        private DateTime dStartedAt;
        private long lngRunningHours;
        private int intRunningMinutes;
        private int intRunningSeconds;
        private MessageHelper _messageHelper;

        private bool fForceNightTime = false;
        private const string C_MODULE_NAME = "MainScreen";

        #endregion

        #region Constructors...
        public TransViewer()
        {
            InitializeComponent();

            _messageHelper = new MessageHelper();
            _messageHelper.CatchedRLAdminMessage += new RLAdminMessageEventHanlder(TransViewer_CatchedAdminMessage);
        }
        #endregion

        #region Properties...
        #endregion

        #region Private Methods...
        private bool IsNightTime()
        {
            DateTime dPMInitialRange;
            DateTime dPMFinalRange;
            DateTime dAMInitialRange;
            DateTime dAMFinalRange;
            DateTime dRightNow;

            if (fForceNightTime | DateTime.Now.DayOfWeek == DayOfWeek.Saturday | DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
                return true;

            dPMInitialRange = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 0, 0, 0);
            dPMFinalRange = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59, 999);
            dAMInitialRange = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0, 0);
            dAMFinalRange = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 6, 59, 59, 999);

            dRightNow = DateTime.Now;

            if (dRightNow >= dPMInitialRange & dRightNow <= dPMFinalRange)
                return true;


            if (dRightNow >= dAMInitialRange & dRightNow <= dAMFinalRange)
                return true;

            return false;
        }

        private void LoadDataTable()
        {
            DataColumn[] TableKey = new DataColumn[1];

            lstData.Columns.Add("Transaction", Type.GetType("System.String"));
            lstData.Columns.Add("Key", Type.GetType("System.Int64"));
            lstData.Columns.Add("Status", Type.GetType("System.Int32"));
            lstData.Columns.Add("TransType", Type.GetType("System.String"));
            TableKey[0] = lstData.Columns["Key"];
            lstData.PrimaryKey = TableKey;
        }

        private void LoadGrid()
        {
            grdTransList.ClearStructure();
            grdTransList.DataSource = lstData;
            grdTransList.RetrieveStructure();

            grdTransList.RootTable.Columns[0].CellStyle.FontName = "Courier New";
            grdTransList.RootTable.Columns[0].Selectable = false;
            grdTransList.RootTable.Columns[0].Width = grdTransList.Width - lngTransColumnPadding;
            grdTransList.RootTable.Columns[1].Visible = false;
            grdTransList.RootTable.Columns[2].Visible = false;
            grdTransList.RootTable.Columns[3].Visible = false;

            //Pending Transaction Format
            GridEXFormatCondition fmtFormatCondition1 = new GridEXFormatCondition();

            fmtFormatCondition1.Column = grdTransList.RootTable.Columns[2];
            fmtFormatCondition1.ConditionOperator = ConditionOperator.Equal;
            fmtFormatCondition1.Value1 = 0;
            fmtFormatCondition1.TargetColumn = grdTransList.RootTable.Columns[0];
            fmtFormatCondition1.Key = "PendingFormat";

            fmtFormatCondition1.FormatStyle.BackColor = Color.Black;
            fmtFormatCondition1.FormatStyle.ForeColor = Color.Yellow;
            fmtFormatCondition1.FormatStyle.FontBold = TriState.True;

            grdTransList.RootTable.FormatConditions.Add(fmtFormatCondition1);


            //Success Transaction Format
            GridEXFormatCondition fmtFormatCondition2 = new GridEXFormatCondition();

            fmtFormatCondition2.Column = grdTransList.RootTable.Columns[2];
            fmtFormatCondition2.ConditionOperator = Janus.Windows.GridEX.ConditionOperator.Equal;
            fmtFormatCondition2.Value1 = 1;
            fmtFormatCondition2.TargetColumn = grdTransList.RootTable.Columns[0];
            fmtFormatCondition2.Key = "SuccessFormat";

            fmtFormatCondition2.FormatStyle.BackColor = Color.Black;
            fmtFormatCondition2.FormatStyle.ForeColor = Color.Chartreuse;
            fmtFormatCondition2.FormatStyle.FontBold = TriState.True;


            grdTransList.RootTable.FormatConditions.Add(fmtFormatCondition2);


            //Fail Transaction Format
            GridEXFormatCondition fmtFormatCondition3 = new GridEXFormatCondition();

            fmtFormatCondition3.Column = grdTransList.RootTable.Columns[2];
            fmtFormatCondition3.ConditionOperator = Janus.Windows.GridEX.ConditionOperator.Equal;
            fmtFormatCondition3.Value1 = 2;
            fmtFormatCondition3.TargetColumn = grdTransList.RootTable.Columns[0];
            fmtFormatCondition3.Key = "FailedFormat";

            fmtFormatCondition3.FormatStyle.BackColor = System.Drawing.Color.Black;
            fmtFormatCondition3.FormatStyle.ForeColor = System.Drawing.Color.Red;
            fmtFormatCondition3.FormatStyle.FontBold = Janus.Windows.GridEX.TriState.True;

            grdTransList.RootTable.FormatConditions.Add(fmtFormatCondition3);

            //Pending Transaction Format
            GridEXFormatCondition fmtFormatCondition4 = new GridEXFormatCondition();

            fmtFormatCondition4.Column = grdTransList.RootTable.Columns[2];
            fmtFormatCondition4.ConditionOperator = Janus.Windows.GridEX.ConditionOperator.Equal;
            fmtFormatCondition4.Value1 = 3;
            fmtFormatCondition4.TargetColumn = grdTransList.RootTable.Columns[0];
            fmtFormatCondition4.Key = "ReversalFormat";

            fmtFormatCondition4.FormatStyle.BackColor = System.Drawing.Color.Black;
            fmtFormatCondition4.FormatStyle.ForeColor = System.Drawing.Color.Orange;
            fmtFormatCondition4.FormatStyle.FontBold = Janus.Windows.GridEX.TriState.True;

            grdTransList.RootTable.FormatConditions.Add(fmtFormatCondition4);

        }

        private void InsertRecord(string strTransaction, long lngTransID,
            int intStatus, string strTransType)
        {
            DataRow itmRow;

            itmRow = lstData.NewRow();
            itmRow["Transaction"] = strTransaction;
            itmRow["Key"] = lngTransID;
            itmRow["Status"] = intStatus;
            itmRow["TransType"] = strTransType;
            lstData.Rows.Add(itmRow);
        }

        private void UpdateTransactionStatus(long lngTransID, TransStatus intStatus)
        {
            DataRow itmRow;

            itmRow = lstData.Rows.Find(lngTransID);

            if (itmRow != null)
            {
                itmRow["Status"] = intStatus;
                itmRow.AcceptChanges();
            }
            else
            {                
                Function.objTransQueue.AddProcessedTransToQueue(lngTransID, intStatus);
                lngRequestedPendingQty += 1;

                switch ((int)intStatus)
                {
                    case 1:
                        lngProcessedOk -= 1;
                        break;

                    case 2:
                        lngProcessedFail -= 1;
                        break;

                    case 3:
                        lngProcessedOk -= 1;
                        break;
                }
            }

        }

        private void GetRequestedTransactions()
        {
            string strTransaction;
            string[] arrTransaction;

            strTransaction = Function.objTransQueue.GetRequestedTransFromQueue();

            if (strTransaction.Trim() != "")
            {
                arrTransaction = strTransaction.Split('|');

                InsertRecord(arrTransaction[0], long.Parse(arrTransaction[1]), int.Parse(arrTransaction[2]), arrTransaction[3]);

                lngRequestedQty += 1;
                lngRequestedPendingQty += 1;

                StatusBar1.Panels[0].Text = "Requests: " + lngRequestedQty;
                StatusBar1.Panels[1].Text = "Pending: " + lngRequestedPendingQty;
            }

            Thread.Sleep(10);

        }


        private void GetProcessedTransactions()
        {
            string strTransaction;
            string[] arrTransaction;

            strTransaction = Function.objTransQueue.GetProcessedTransFromQueue();

            if (strTransaction.Trim() != "")
            {
                arrTransaction = strTransaction.Split('|');

                EnumConverter ec = new EnumConverter(typeof(TransStatus));
                TransStatus intStatus = (TransStatus)ec.ConvertFromString(arrTransaction[1]);

                UpdateTransactionStatus(long.Parse(arrTransaction[0]), intStatus);
                
                lngRequestedPendingQty -= 1;

                switch (intStatus)
                {
                    case TransStatus.tsSuccess:
                        lngProcessedOk += 1;
                        break;

                    case TransStatus.tsFail:
                        lngProcessedFail += 1;
                        break;

                    case TransStatus.tsReversal:
                        lngProcessedOk += 1;
                        break;
                }

                StatusBar1.Panels[1].Text = "Pending: " + lngRequestedPendingQty;
                StatusBar1.Panels[2].Text = "Success: " + lngProcessedOk;
                StatusBar1.Panels[3].Text = "Fail: " + lngProcessedFail;

                Thread.Sleep(10);

            }

        }

        private void ShrinkRecordset()
        {
            if (lstData.Rows.Count > lngMaxRecordsQuantity)
                lstData.Rows.RemoveAt(0);
        }

        private void GetTransactionsOnQueue()
        {
            long lngTransactionsOnQueue;

            lngTransactionsOnQueue = Function.objRequestQueue.QueuedRequests;

            StatusBar1.Panels[4].Text = "On Queue: " + lngTransactionsOnQueue.ToString();

        }

        private void GetWorkingThreads()
        {
            int intWorkingThreads;

            intWorkingThreads = Function.objThreadController.WorkingThreads;

            StatusBar1.Panels[5].Text = "Threads: " + intWorkingThreads.ToString();

        }

        private void GetOpenedSockets()
        {
            int intOpenedSockets;

            intOpenedSockets = Function.objListener.OpenedSockets;

            StatusBar1.Panels[6].Text = "Sockets: " + intOpenedSockets.ToString();

        }
        #endregion

        #region Event Handlers...
        private void chkForceNightTime_CheckedChanged(object sender, EventArgs e)
        {
            fForceNightTime = chkForceNightTime.Checked;
        }

        private void TransViewer_Load(object sender, EventArgs e)
        {
           Text = "SMS -  Ver. " + Application.ProductVersion + " ... port[" + Config.Value("port") + "]";
#if LOG
            //Function.objLogWriter.Append("Starting application ...", C_MODULE_NAME);
#endif
            try
            {
                LoadDataTable();
                LoadGrid();

                lngRequestedQty = 0;
                lngRequestedPendingQty = 0;
                lngProcessedOk = 0;
                lngProcessedFail = 0;
                dStartedAt = DateTime.Now;

                StatusBar1.Panels[0].Text = "Requests: 0";
                StatusBar1.Panels[1].Text = "Pending: 0";
                StatusBar1.Panels[2].Text = "Success: 0";
                StatusBar1.Panels[3].Text = "Fail: 0";
                StatusBar1.Panels[4].Text = "On Queue: 0";
                StatusBar1.Panels[5].Text = "Threads: 0";
                StatusBar1.Panels[6].Text = "Sockets: 0";
                StatusBar1.Panels[7].Text = "Started at " + dStartedAt.ToShortDateString() + " " + dStartedAt.ToShortTimeString();
                StatusBar1.Panels[8].Text = "Running Time: 00:00:00";

                Function.objListener = new Listener();
#if LOG
                Function.objLogWriter.Append("Application was started.", C_MODULE_NAME);
#endif

                if (Config.Status != "OK")
                    lstData.Rows.Add(Config.Status);
            }
            catch (Exception ex)
            {
#if LOG
                Function.objLogWriter.Append(ex.Message, C_MODULE_NAME);
#endif
            }
        }

        private void TransViewer_CatchedAdminMessage(RLAdminMessageType type)
        {
            switch (type)
            { 
                case RLAdminMessageType.Close:
                    TransViewer_FormClosing(this, 
                        new FormClosingEventArgs(CloseReason.ApplicationExitCall, false));
                    break;
            }
        }

        private void TransViewer_FormClosing(object sender, FormClosingEventArgs e)
        {
            //Do not get more transactions
#if LOG
            Function.objLogWriter.Append("Pausing Listener ...", C_MODULE_NAME);
#endif
            Function.objListener.PauseListener();
#if LOG
            Function.objLogWriter.Append("Listener Paused.", C_MODULE_NAME);
#endif
            //Finish with pending transactions
#if LOG
            Function.objLogWriter.Append("Finishing pending transactions ...", C_MODULE_NAME);
#endif
            Function.objThreadController.ResumeWork();
            while (Function.objThreadController.WorkingThreads > 0)
                Thread.Sleep(10);
#if LOG
            Function.objLogWriter.Append("Pending transactions Finished.", C_MODULE_NAME);
#endif
            //Disconect Listener
#if LOG
            Function.objLogWriter.Append("Disconecting Listener ...", C_MODULE_NAME);
#endif
            Function.objListener.Disconnect();
#if LOG
            Function.objLogWriter.Append("Listener disconected.", C_MODULE_NAME);
#endif
        }

        private void TransViewer_Resize(object sender, EventArgs e)
        {
            grdTransList.RootTable.Columns[0].Width = grdTransList.Width - lngTransColumnPadding;
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            Timer1.Enabled = false;

            string method = "";
            try
            {
                if (IsNightTime())
                {
                    Label4.Text = "Night Time On";
                    Label4.BackColor = Color.Navy;
                    Label4.ForeColor = Color.White;
                }
                else
                {
                    Label4.Text = "Night Time Off";
                    Label4.BackColor = Color.White;
                    Label4.ForeColor = Color.Navy;
                }

                //Refresh Requested Transactions
                method = "GetRequestedTransactions";
                GetRequestedTransactions();

                //Refresh Processed Transactions
                method = "GetProcessedTransactions";
                GetProcessedTransactions();

                //Refresh Transactions On Queue
                method = "GetTransactionsOnQueue";
                GetTransactionsOnQueue();

                //Refresh Working Threads
                method = "GetWorkingThreads";
                GetWorkingThreads();

                //Refresh Current Sockets
                method = "GetOpenedSockets";
                GetOpenedSockets();

                //Shrink recordset
                method = "ShrinkRecordset";
                ShrinkRecordset();
                if (grdTransList.RowCount > 0)
                    grdTransList.Row = grdTransList.RowCount - 1;
            }
            catch (Exception ex)
            {
#if LOG
                Function.objLogWriter.Append(ex.Message + ". " + method, C_MODULE_NAME);
#endif
            }
            finally
            {
                Thread.Sleep(10);
                Timer1.Enabled = true;
            }
        }

        private void Timer2_Tick(object sender, EventArgs e)
        {
            intRunningSeconds += 1;
            if (intRunningSeconds == 60)
            {
                intRunningSeconds = 0;

                intRunningMinutes += 1;
                if (intRunningMinutes == 60)
                {
                    intRunningMinutes = 0;
                    lngRunningHours += 1;
                }
            }

            StatusBar1.Panels[8].Text = "Running Time: " +
                    lngRunningHours.ToString().PadLeft(2, '0') + ":" +
                    intRunningMinutes.ToString().PadLeft(2, '0') + ":" +
                    intRunningSeconds.ToString().PadLeft(2, '0');

        }
        #endregion
    }
}