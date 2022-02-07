using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;

namespace RequestListener
{
    class Request
    {
        #region Fields...
        private Socket objSocket;
        private long lngSocketID;
        private long lngSocketTransID;
        private DateTime dSocketTimeIn;
        private int intSpliterTransID;
        private DateTime dRequestQueueIn;
        private DateTime dRequestQueueOut;
        private string strRequest;
        #endregion

        #region Constructors...
        public Request(Socket objSocketA,
                    long lngSocketIDA,
                    long lngSocketTransIDA,
                    DateTime dSocketTimeInA,
                    int intSpliterTransIDA,
                    string strRequestA)
        {

            objSocket = objSocketA;
            lngSocketID = lngSocketIDA;
            lngSocketTransID = lngSocketTransIDA;
            dSocketTimeIn = dSocketTimeInA;
            intSpliterTransID = intSpliterTransIDA;
            strRequest = strRequestA;
        }
        #endregion

        #region Properties...
        public Socket RequestSocket
        {
            get { return objSocket; }
        }

        public long RequestSocketID
        {
            get { return lngSocketID; }
        }

        public long SocketTransID
        {
            get { return lngSocketTransID; }
        }

        public DateTime SocketTimeIn
        {
            get { return dSocketTimeIn; }
        }

        public int SpliterTransID
        {
            get { return intSpliterTransID; }
        }

        public string Description
        {
            get { return strRequest; }
        }

        public DateTime RequestQueueIn
        {
            get { return dRequestQueueIn; }
            set { dRequestQueueIn = value; }
        }

        public DateTime RequestQueueOut
        {
            get { return dRequestQueueOut; }
            set { dRequestQueueOut = value; }
        }
        #endregion

        #region Private Methods...
        #endregion

        #region Exposed Methods...
        #endregion

    }
}
