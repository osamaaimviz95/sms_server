using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Net.Sockets;

namespace RequestListener
{
    public enum ReaderStatus
    {
        rsStopped = 0,
        rsReading = 1,
    }

    class Reader
    {
        #region Fields...
        private Socket objSocket;

        private long lngSocketID;
        private long lngTransactionID = 0;
        private byte[] bytRequest = new byte[READ_BUFFER_SIZE];

        private const int READ_BUFFER_SIZE = 4095;
        private const string C_MODULE_NAME = "Reader";
        #endregion

        #region Constructors...
        public Reader(Socket objSocketA, long lngAcceptedSocketID)
        {
            Thread objReaderThread;
            ListenerStatus intListenerStatus;

            //Get SocketID
            lngSocketID = lngAcceptedSocketID;

            Function.objListener.IncrementSocketsQuantity();

            intListenerStatus = Function.objListener.Status;

            if (intListenerStatus == ListenerStatus.lsListening)
            {
                //Get Socket Reference
                objSocket = objSocketA;

                //Start new thread
                objReaderThread = new Thread(new ThreadStart(StartReading));
                objReaderThread.Start();
            }
            else
            {
                Function.objListener.DecrementSocketsQuantity();
            }
        }
        #endregion

        #region Properties...
        #endregion

        #region Private Methods...
        private void StartReading()
        {
            int BytesRead = 0;
            DateTime dTransTimeIn;
            ListenerStatus intListenerStatus;

            try
            {
                //Receive Request
/*#if LOG
                Function.objLogWriter.Append(lngSocketID, 0, 0, 0, "Beginning to receive requests from socket ...", C_MODULE_NAME);
#endif*/
                objSocket.Blocking = false;

                do
                {
                    if (objSocket.Available > 0)
                    {
                        lngTransactionID += 1;
                        dTransTimeIn = DateTime.Now;
/*#if LOG
                        Function.objLogWriter.Append(lngSocketID, lngTransactionID, 0, 0, "Receiving request from socket ...", C_MODULE_NAME);
#endif*/
                        try
                        {
                            BytesRead = objSocket.Receive(bytRequest, 0, READ_BUFFER_SIZE, SocketFlags.None);
                        }
                        catch { }
#if LOG
                        Function.objLogWriter.Append(lngSocketID, lngTransactionID, 0, 0, BytesRead + " bytes were received from socket.", C_MODULE_NAME);
#endif
                        if (BytesRead > 0)
                        {
                            //Send Request to Request Spliter
/*#if LOG
                            Function.objLogWriter.Append(lngSocketID, lngTransactionID, 0, 0, "Sending request to Request Spliter ...", C_MODULE_NAME);
#endif*/
                            RequestSpliter objRequestSpliter = new RequestSpliter(objSocket,
                                                                        bytRequest,
                                                                        BytesRead,
                                                                        lngSocketID,
                                                                        lngTransactionID,
                                                                        dTransTimeIn);
/*#if LOG
                            Function.objLogWriter.Append(lngSocketID, lngTransactionID, 0, 0, "Request was sent to Request Spliter.", C_MODULE_NAME);
#endif*/
                            System.Array.Clear(bytRequest, 0, bytRequest.Length);
                            BytesRead = 0;

                        }
                    }

                    intListenerStatus = Function.objListener.Status;

                    Thread.Sleep(125);
                }while (intListenerStatus != ListenerStatus.lsPaused && intListenerStatus != ListenerStatus.lsStopped);

#if LOG
                Function.objLogWriter.Append(lngSocketID, 0, 0, 0, "Either the Listener was paused or stopped.", C_MODULE_NAME);
                
                Function.objLogWriter.Append(lngSocketID, 0, 0, 0, "Changing reader status to stopped.", C_MODULE_NAME);
                Function.objLogWriter.Append(lngSocketID, 0, 0, 0, "Reader status was changed to stopped.", C_MODULE_NAME);
#endif
                //objSocket.Shutdown(SocketShutdown.Receive)
            }
            catch (Exception ex)
            {
#if LOG
                Function.objLogWriter.Append(lngSocketID, lngTransactionID, 0, 0, ex.Message, C_MODULE_NAME);
#endif
            }
            finally
            {
                intListenerStatus = Function.objListener.Status;

                if (intListenerStatus == ListenerStatus.lsPaused)
                {
#if LOG
                    Function.objLogWriter.Append(lngSocketID, 0, 0, 0, "Waiting until Listener status is stopped", C_MODULE_NAME);
#endif
                    do
                    {
                        Thread.Sleep(10);
                        intListenerStatus = Function.objListener.Status;
                    }
                    while (intListenerStatus != ListenerStatus.lsStopped);
#if LOG
                    Function.objLogWriter.Append(lngSocketID, 0, 0, 0, "Listener status is stopped.", C_MODULE_NAME);
#endif
                    }

                    if (objSocket.Connected)
                    {
                        Function.objListener.DecrementSocketsQuantity();

#if LOG
                        Function.objLogWriter.Append(lngSocketID, 0, 0, 0, "Shutting down socket ...", C_MODULE_NAME);
#endif
                        objSocket.Shutdown(SocketShutdown.Both);
                        objSocket.Close();
#if LOG
                        Function.objLogWriter.Append(lngSocketID, 0, 0, 0, "Socket was shut down.", C_MODULE_NAME);
#endif
                    }
            }

        }
        #endregion

        #region Exposed Methods...
        #endregion
    }
}
