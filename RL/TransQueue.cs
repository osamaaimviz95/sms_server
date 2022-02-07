using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Collections;

namespace RequestListener
{
    public enum TransStatus
    {
        tsPending = 0,
        tsSuccess = 1,
        tsFail = 2,
        tsReversal = 3
    }

    class TransQueue
    {
        #region Fields...
        private long lngTransactionID = 0;
        private Queue<string> qRequestsQueue;
        private Queue<string> qProcessedQueue;
        private object sync;
        #endregion

        #region Constructors...
        public TransQueue()
        {
            qRequestsQueue = new Queue<string>();
            qProcessedQueue = new Queue<string>();

            sync = new object();
        }
        #endregion

        #region Properties...
        #endregion

        #region Private Methods...
        #endregion

        #region Exposed Methods...
        public long AddRequestedTransToQueue(string strTransaction, string strTransactionType)
        {
            TransStatus intTransStatus;

            lock (sync)
            {
                intTransStatus = TransStatus.tsPending;

                Interlocked.Increment(ref lngTransactionID);
                strTransaction += "|" + lngTransactionID;
                strTransaction += "|" + (int)intTransStatus;
                strTransaction += "|" + strTransactionType;
                qRequestsQueue.Enqueue(strTransaction);

                return lngTransactionID;
            }
        }

        public void AddProcessedTransToQueue(long lngTransID, TransStatus intResult)
        {
            string strTransaction;

            lock (sync)
            {
                strTransaction = "";
                strTransaction += lngTransID;
                strTransaction += "|" + intResult;
                qProcessedQueue.Enqueue(strTransaction);
            }
        }

        public string GetRequestedTransFromQueue()
        {
            lock (sync)
            {
                if (qRequestsQueue.Count > 0)
                    return qRequestsQueue.Dequeue();
                else
                    return "";
            }
        }

        public string GetProcessedTransFromQueue()
        {
            lock (sync)
            {
                if (qProcessedQueue.Count > 0)
                    return qProcessedQueue.Dequeue();
                else
                    return "";
            }
        }
        #endregion
    }
}
