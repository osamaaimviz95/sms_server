using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using Common;

namespace RequestListener
{
    public enum ListenerStatus
    {
        lsStopped = 0,
        lsListening = 1,
        lsPaused = 2
    }

    class Listener
    {
        #region Fields...
        //Socket Variables
        private int intPortNumber = 6000;
        private TcpListener objListener;
        private ListenerStatus lsCurrentStatus;

        //Other Variables
        private long lngAcceptedSocketID = 0;
        private int intOpenedSockets = 0;
        private object objSyncRoot = new object();

        private const string C_MODULE_NAME = "Listener";
        #endregion

        #region Constructors...
        public Listener()
        {
            Thread objListenerThread = new Thread(new ThreadStart(Connect));
#if LOG
            Function.objLogWriter.Append("Starting listener thread ...", C_MODULE_NAME);
#endif
            lsCurrentStatus = ListenerStatus.lsStopped;
            objListenerThread.Start();
#if LOG
            Function.objLogWriter.Append("Listener thread started.", C_MODULE_NAME);
#endif
        }
        #endregion

        #region Properties...
        public ListenerStatus Status
        {
            get
            {
                lock (objSyncRoot)
                {
                    return lsCurrentStatus;
                }
            }
        }

        public int OpenedSockets
        {
            get
            {
                lock (objSyncRoot)
                {
                    return intOpenedSockets;
                }
            }
        }

        #endregion

        #region Private Methods...
        private void Connect()
        {
            try
            {
#if LOG
                Function.objLogWriter.Append("Beginning to listen on port " + intPortNumber + " ...", C_MODULE_NAME);
#endif
                //Listen on port
                intPortNumber = Int32.Parse(Config.Value("port"));
                //objListener = new TcpListener(IPAddress.Parse("192.168.1.50"), intPortNumber);
                objListener = new TcpListener(intPortNumber);
                objListener.Start();
#if LOG
                Function.objLogWriter.Append("Listening on port " + intPortNumber + ".", C_MODULE_NAME);
#endif
                lock (objSyncRoot)
                {
                    //Update Status
                    lsCurrentStatus = ListenerStatus.lsListening;
                }

                do
                {
                    if (lsCurrentStatus == ListenerStatus.lsListening)
                    {
                        lngAcceptedSocketID += 1;

                        //Accept Socket
/*#if LOG
                        Function.objLogWriter.Append(lngAcceptedSocketID, 0, 0, 0, "Accepting Socket ...", C_MODULE_NAME);
#endif*/
                        Reader objReader = new Reader(objListener.AcceptSocket(), lngAcceptedSocketID);
/*#if LOG
                        Function.objLogWriter.Append(lngAcceptedSocketID, 0, 0, 0, "Socket accepted.", C_MODULE_NAME);
#endif*/
                    }
                }
                while (lsCurrentStatus != ListenerStatus.lsStopped);
            }
            catch (Exception ex)
            {
#if LOG
                Function.objLogWriter.Append(lngAcceptedSocketID, 0, 0, 0, ex.Message, C_MODULE_NAME);
#endif
            }
        }
        #endregion

        #region Exposed Methods...
        public void PauseListener()
        {
#if LOG
            Function.objLogWriter.Append("Changing status to paused ...", C_MODULE_NAME);
#endif
            lock (objSyncRoot)
            {
                //Update Status
                lsCurrentStatus = ListenerStatus.lsPaused;
            }
#if LOG
            Function.objLogWriter.Append("Changing status to paused ...", C_MODULE_NAME);
#endif
        }

        public void Disconnect()
        {
#if LOG
            Function.objLogWriter.Append("Changing status to stopped ...", C_MODULE_NAME);
#endif
            lock (objSyncRoot)
            {
                //Update Status
                lsCurrentStatus = ListenerStatus.lsStopped;
            }
#if LOG
            Function.objLogWriter.Append("Status was changed to stopped ...", C_MODULE_NAME);
#endif
            //Wait while Sockets are still active
#if LOG
            Function.objLogWriter.Append("Wait until all sockets finish their job ...", C_MODULE_NAME);
#endif
            while (intOpenedSockets > 0)
                Thread.Sleep(10);
#if LOG
            Function.objLogWriter.Append("All sockets finished their job.", C_MODULE_NAME);
#endif
            //Disconnect listener
#if LOG
            Function.objLogWriter.Append("Disconnecting from port " + intPortNumber + " ...", C_MODULE_NAME);
#endif
            objListener.Stop();
#if LOG
            Function.objLogWriter.Append("Disconnected from port " + intPortNumber + ".", C_MODULE_NAME);
#endif
        }

        public void IncrementSocketsQuantity()
        {
            lock (objSyncRoot)
            {
#if LOG
                Function.objLogWriter.Append("Incrementing sockets. Current quantity: " + intOpenedSockets, C_MODULE_NAME);
#endif
                intOpenedSockets++;
#if LOG
                Function.objLogWriter.Append("Incrementing sockets. New quantity: " + intOpenedSockets, C_MODULE_NAME);
#endif
            }
        }

        public void DecrementSocketsQuantity()
        {
            lock (objSyncRoot)
            {
#if LOG
                Function.objLogWriter.Append("Decrementing sockets. Current quantity: " + intOpenedSockets, C_MODULE_NAME);
#endif
                intOpenedSockets--;
#if LOG
                Function.objLogWriter.Append("Decrementing sockets. New quantity: " + intOpenedSockets, C_MODULE_NAME);
#endif
            }
        }
        #endregion
    }
}
