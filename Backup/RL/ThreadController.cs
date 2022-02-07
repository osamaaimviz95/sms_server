using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace RequestListener
{
    class ThreadController
    {
        #region Fields...
        private object objSyncRoot;
        private long lngTransactionID;
        private int intWorkingThreads;
        private bool fResumeWork;
        private DateTime dThreadContOut;

        private const int MAX_THREADS_QTY = 10;
        private const string C_MODULE_NAME = "ThreadController";
        #endregion

        #region Constructors...
        public ThreadController()
        {
            objSyncRoot = new object();
            intWorkingThreads = 0;
            fResumeWork = false;
#if LOG
            Function.objLogWriter.Append("Starting Thread Controller thread ...", C_MODULE_NAME);
#endif
            Thread newThread = new Thread(new ThreadStart(ReadFromQueue));
#if LOG
            Function.objLogWriter.Append("Thread Controller thread was started.", C_MODULE_NAME);
#endif
            newThread.Start();
        }
        #endregion

        #region Properties...
        public int WorkingThreads
        {
            get
            {
                lock (objSyncRoot)
                {
                    return intWorkingThreads;
                }
            }
        }
        #endregion

        #region Private Methods...
        private void ReadFromQueue()
        {
            Request objRequest;
            bool fContinue = true;

            while (fContinue | Function.objRequestQueue.QueuedRequests > 0)
            {
                objRequest = Function.objRequestQueue.GetNextRequest();
                if (objRequest != null)
                {
                    lngTransactionID ++;
/*#if LOG
                    Function.objLogWriter.Append(objRequest.RequestSocketID, objRequest.SocketTransID, objRequest.SpliterTransID, lngTransactionID, "Request gotten from Requests Queue ...", C_MODULE_NAME);
#endif*/
                    if (((TimeSpan)DateTime.Now.Subtract(objRequest.SocketTimeIn)).Seconds < 25)
                    {
                        ProcessRequest(objRequest);
                    }
                    else
                    {
#if LOG
                        Function.objLogWriter.Append(objRequest.RequestSocketID, objRequest.SocketTransID, objRequest.SpliterTransID, lngTransactionID, "Transaction was not processed. It was 25 seconds or more on the queue waiting to be processed.", C_MODULE_NAME);
#endif
                    }
                }
                else
                {
                    fContinue = !fResumeWork;
                }

                Thread.Sleep(10);
            }
#if LOG
            Function.objLogWriter.Append("Exiting from Read Queue Loop. Last Transaction Processed: " + lngTransactionID, C_MODULE_NAME);
#endif
        }

        private void ProcessRequest(Request objRequest)
        {
            Processor objProcessor;
            Thread objThread;
            int intCurrentWT;
/*#if LOG
            Function.objLogWriter.Append(objRequest.RequestSocketID, objRequest.SocketTransID, objRequest.SpliterTransID, lngTransactionID, "Sending request to Processor ...", C_MODULE_NAME);
#endif*/
            dThreadContOut = DateTime.Now;

            objProcessor = new Processor(objRequest.RequestSocket,
                                    objRequest.RequestSocketID,
                                    objRequest.SocketTransID,
                                    objRequest.SocketTimeIn,
                                    objRequest.SpliterTransID,
                                    objRequest.RequestQueueIn,
                                    objRequest.RequestQueueOut,
                                    objRequest.Description,
                                    lngTransactionID,
                                    dThreadContOut
                                    );

            //jctb 07282005
            objThread = new Thread(new ThreadStart(objProcessor.ProcessRequest));
            Function.objWorkingThreadsEvHdl.Reset();
            objThread.Start();

            //Increment Working Threads
            lock (objSyncRoot)
            {
                intWorkingThreads += 1;
                intCurrentWT = intWorkingThreads;
            }

            //Check If another thread can be created
            while (intCurrentWT >= MAX_THREADS_QTY)
            {
#if LOG
                Function.objLogWriter.Append(objRequest.RequestSocketID, objRequest.SocketTransID, objRequest.SpliterTransID, lngTransactionID, "Waiting for available thread ... Pending Requests: " + Function.objRequestQueue.QueuedRequests, C_MODULE_NAME);
#endif
                Function.objWorkingThreadsEvHdl.WaitOne();
#if LOG
                Function.objLogWriter.Append(objRequest.RequestSocketID, objRequest.SocketTransID, objRequest.SpliterTransID, lngTransactionID, "Ready to process next transaction.", C_MODULE_NAME);
#endif
                //Get Current Working Threads
                lock (objSyncRoot)
                {
                    intCurrentWT = intWorkingThreads;
                }

            }
        }
        #endregion

        #region Exposed Methods...
        public void DecrementThreadsQuantity(long lngRequestSocketID,
                                        long lngSocketTransID,
                                        long lngSpliterTransID,
                                        long lngThreadContTransID)
        {

            lock (objSyncRoot)
            {
#if LOG
                Function.objLogWriter.Append(lngRequestSocketID, lngSocketTransID, lngSpliterTransID, lngThreadContTransID, "Decrementing Threads Quantity. Old Value: " + intWorkingThreads.ToString(), C_MODULE_NAME);
#endif
                intWorkingThreads --;
#if LOG
                Function.objLogWriter.Append(lngRequestSocketID, lngSocketTransID, lngSpliterTransID, lngThreadContTransID, "Decrementing Threads Quantity. New Value: " + intWorkingThreads.ToString(), C_MODULE_NAME);
#endif
            }
        }

        public void ResumeWork()
        {
            lock (objSyncRoot)
            {
                fResumeWork = true;
                Function.objLogWriter.Append("Resume Work Flag was activated. Pending Requests: " + Function.objRequestQueue.QueuedRequests, C_MODULE_NAME);
            }
        }
        #endregion
    }
}
