using System;
using System.Collections.Generic;
using System.Text;

namespace RequestListener
{
    class RequestQueue
    {
        #region Fields...
        private Queue<Request> objRequestQueue;
        private object sync;

        private const string C_MODULE_NAME = "RequestQueue";
        #endregion

        #region Consturctors...
        public RequestQueue()
        {
#if LOG
            Function.objLogWriter.Append("Initializing Request Queue ...", C_MODULE_NAME);
#endif
            objRequestQueue = new Queue<Request>();
#if LOG
            Function.objLogWriter.Append("Request Queue was initialized.", C_MODULE_NAME);
#endif
            sync = new object();
        }
        #endregion

        #region Properties...
        public int QueuedRequests
        {
            get
            {
                int intQueuedRequests;

                lock (sync)
                {
                    intQueuedRequests = objRequestQueue.Count;
                }

                return intQueuedRequests;
            }
        }
        #endregion

        #region Private Methods...
        #endregion

        #region Exposed Methods...
        public void EnqueueRequest(Request objRequest)
        {
            lock (sync)
            {
/*#if LOG
                Function.objLogWriter.Append(objRequest.RequestSocketID, objRequest.SocketTransID, objRequest.SpliterTransID, 0, "Adding Request to Queue ...", C_MODULE_NAME);
#endif*/
                objRequest.RequestQueueIn = DateTime.Now;
                objRequestQueue.Enqueue(objRequest);
/*#if LOG
                Function.objLogWriter.Append(objRequest.RequestSocketID, objRequest.SocketTransID, objRequest.SpliterTransID, 0, "Request was added to Queue.", C_MODULE_NAME);
#endif*/
            }
        }

        public Request GetNextRequest()
        {
            Request objRequest = null;

            lock (sync)
            {
                if (objRequestQueue.Count > 0)
                {
                    objRequest = objRequestQueue.Dequeue();
                    objRequest.RequestQueueOut = DateTime.Now;
/*#if LOG
                    Function.objLogWriter.Append(objRequest.RequestSocketID, objRequest.SocketTransID, objRequest.SpliterTransID, 0, "Request was sent to Thread Controller.", C_MODULE_NAME);
#endif*/
                }
            }
            return objRequest;
        }
        #endregion
    }
}
