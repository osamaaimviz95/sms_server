using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using FunctionDispatcher;
using Common;

namespace RequestListener
{
    class Processor
    {
        #region Fields...
        //Control Variables
        private string strRequest;
        private Response strResponse;
        private Socket objSocket;
        private long lngSocketID;
        private long lngTransactionID;
        private long lngSocketTransID;
        private DateTime dSocketTimeIn;
        private long lngSpliterTransID;
        private DateTime dRequestQueueIn;
        private DateTime dRequestQueueOut;
        private DateTime dThreadContOut;

        //RequestLog Fields
        private string strTransactionType;
        private DateTime dTimeIn;
        private DateTime dTimeOut;

        private const string C_MODULE_NAME = "Processor";
        #endregion

        #region Constructors...
        public Processor(Socket objSocketA,
                    long lngSocketIDA,
                    long lngSocketTransIDA,
                    DateTime dSocketTimeInA,
                    long lngSpliterTransIDA,
                    DateTime dRequestQueueInA,
                    DateTime dRequestQueueOutA,
                    string strRequestA,
                    long lngTransactionIDA,
                    DateTime dThreadContOutA)
        {

            objSocket = objSocketA;
            lngSocketID = lngSocketIDA;
            lngSocketTransID = lngSocketTransIDA;
            dSocketTimeIn = dSocketTimeInA;
            lngSpliterTransID = lngSpliterTransIDA;
            dRequestQueueIn = dRequestQueueInA;
            dRequestQueueOut = dRequestQueueOutA;
            strRequest = strRequestA;
            lngTransactionID = lngTransactionIDA;
            dThreadContOut = dThreadContOutA;
        }
        #endregion

        #region Properties...
        #endregion

        #region Private Methods...
        private long UpdateRequestsQueue()
        {
            return UpdateRequestsQueue(false, "");
        }

        private long UpdateRequestsQueue(bool fError, string strErrorMessage)
        {
            string strTransRequest;
            long lngQueueTransID;

            if (!fError)
                strTransRequest = "RQ:" + DateTime.Now.ToString("yyMMdd hh:mm:ss:fff")+ " " + strRequest;
            else
                strTransRequest = "Error: " + strErrorMessage;

            strTransactionType = "  ";
            lngQueueTransID = Function.objTransQueue.AddRequestedTransToQueue(strTransRequest, strTransactionType.Substring(0, 2));

            return lngQueueTransID;
        }

        private void UpdateProcessedQueue(long lngQueueTransID, bool fTransOk)
        {
            TransStatus lngTransResult;

            if (fTransOk)
            {
                if (strTransactionType == "R")
                    lngTransResult = TransStatus.tsReversal;
                else
                    lngTransResult = TransStatus.tsSuccess;
            }
            else
                lngTransResult = TransStatus.tsFail;

            Function.objTransQueue.AddProcessedTransToQueue(lngQueueTransID, lngTransResult);
        }

        private byte[] GetResponseBytes()
        {
            //byte[] ResponseBytes = new byte[strResponse.Length - 1];

            //for (int intIndex = 0; intIndex <= strResponse.Length - 1; intIndex++)
            //    ResponseBytes[intIndex] = byte.Parse(strResponse.Substring(intIndex, 1));

            System.Text.Encoding ascii = System.Text.Encoding.ASCII;
            byte[] ResponseBytes = ascii.GetBytes(strResponse.Data);

            return ResponseBytes;
        }

        #endregion

        #region Exposed Methods...
        public void ProcessRequest()
        {
            byte[] bytResponse = null;
            long lngQueueTransID;
            string strHexResponse;
            bool sendFlag = false;


            dTimeIn = DateTime.Now;

            try
            {
                lngQueueTransID = UpdateRequestsQueue();

                strResponse = Function.FunctionDispatcher.Request(strRequest);
                //Convert response to byte array
                strHexResponse = strResponse.Data;
                bytResponse = GetResponseBytes();

                dTimeOut = DateTime.Now;


                //Update Processed Queue
                #if LOG
                                Function.objLogWriter.Append(lngSocketID, lngSocketTransID, lngSpliterTransID, lngTransactionID, "Sending processed request to screen transactions queue ...", C_MODULE_NAME);
                                Function.objLogWriter.Append(lngSocketID, lngSocketTransID, lngSpliterTransID, lngTransactionID, strResponse.Data, C_MODULE_NAME);
                
#endif
                bool ftransOk = true;
                if (strResponse.Status != "1")
                {
                    ftransOk = false;
                }

                UpdateProcessedQueue(lngQueueTransID, ftransOk);
                //#if LOG
                //Function.objLogWriter.Append(lngSocketID, lngSocketTransID, lngSpliterTransID, lngTransactionID, "Processed request was sent to screen transactions queue.", C_MODULE_NAME);
                //#endif
                //Send Response only if transaction time is lower or equal than 25 seconds
                //if (((TimeSpan)DateTime.Now.Subtract(dSocketTimeIn)).Seconds < 25)
                //{
#if LOG
                Function.objLogWriter.Append(lngSocketID, lngSocketTransID, lngSpliterTransID, lngTransactionID, "Sending response to socket ...", C_MODULE_NAME);
#endif

                objSocket.Send(bytResponse, 0, bytResponse.Length, SocketFlags.None);
                sendFlag = true;
                objSocket.Shutdown(SocketShutdown.Both);
                objSocket.Close();
                Function.objListener.DecrementSocketsQuantity();

#if LOG
                Function.objLogWriter.Append(lngSocketID, lngSocketTransID, lngSpliterTransID, lngTransactionID, "Response was sent to socket.", C_MODULE_NAME);
#endif
                //                }
                //                else
                //                {
                //#if LOG
                //                    Function.objLogWriter.Append(lngSocketID, lngSocketTransID, lngSpliterTransID, lngTransactionID, "Transaction was not sent because of timeout.", C_MODULE_NAME);
                //#endif
                //                }

            }
            catch (SocketException ex)
            {
                //Send Error
                if (!sendFlag)
                {
                    Notificator.Send("Socket Error. SocketID=" + lngSocketID + "SocketTransID=" + lngSocketTransID + "SpliterTransID=" + lngSpliterTransID + "lngTransactionID=" + lngTransactionID);
                    Function.objLogWriter.Append(lngSocketID, lngSocketTransID, lngSpliterTransID, lngTransactionID, "Socket Error: " + Convert.ToString(bytResponse), C_MODULE_NAME);
                }
                //Disconnect Error
                else
                {
#if LOG
                    Function.objLogWriter.Append(lngSocketID, lngSocketTransID, lngSpliterTransID, lngTransactionID, ex.Message, C_MODULE_NAME);
#endif
                }
            }
            catch (Exception ex)
            {
#if LOG
                Function.objLogWriter.Append(lngSocketID, lngSocketTransID, lngSpliterTransID, lngTransactionID, ex.Message + ". Request: " + strRequest, C_MODULE_NAME);
#endif
            }
            finally
            { 
                //Decrement Threads Quantity
                Function.objThreadController.DecrementThreadsQuantity(lngSocketID, 
                    lngSocketTransID, lngSpliterTransID, lngTransactionID);

                Function.objWorkingThreadsEvHdl.Set();
            }
        }
        #endregion
    }
}
