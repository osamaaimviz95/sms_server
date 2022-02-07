using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Net.Sockets;

namespace RequestListener
{
    class RequestSpliter
    {
        #region Fields...
        private Socket objSocket;
        private long lngSocketID;
        private long lngSocketTransID;
        private byte[] bytTotalRequest;
        private int intTotalBytesReceived;
        private DateTime dTransTimeIn;

        private const string C_MODULE_NAME = "RequestSpliter";
        #endregion

        #region Constructors...
        protected string PrintBytes(byte[] bytes, int length)
        {
            try
            {
                string result = bytes[0] + "";
                for (int i = 1; i < length; i++)
                    result += " " + bytes[i];

                return result;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
        public RequestSpliter(Socket objSocketA,
                    byte[] bytRequest,
                    int intBytesReceived,
                    long lngAcceptedSocketID,
                    long lngSocketTransIDA,
                    DateTime dTransTimeInA)
        {
/*#if LOG
            Function.objLogWriter.Append(lngAcceptedSocketID, lngSocketTransIDA, 0, 0, "Receiving request data from reader ...", C_MODULE_NAME);
#endif

#if LOG
            Function.objLogWriter.Append(lngAcceptedSocketID, lngSocketTransIDA, 0, 0, Encoding.ASCII.GetString(bytRequest,0,intBytesReceived), C_MODULE_NAME);
#endif*/

#if LOG
            Function.objLogWriter.Append(lngAcceptedSocketID, lngSocketTransIDA, 0, 0, "Byte sequence: " + PrintBytes(bytRequest, intBytesReceived), C_MODULE_NAME);
#endif

            objSocket = objSocketA;
            //bytTotalRequest = Array.CreateInstance(Type.GetType("System.Byte"), intBytesReceived);
            bytTotalRequest = new byte[intBytesReceived];
            System.Array.Copy(bytRequest, bytTotalRequest, intBytesReceived);
            intTotalBytesReceived = intBytesReceived;
            lngSocketID = lngAcceptedSocketID;
            lngSocketTransID = lngSocketTransIDA;
            dTransTimeIn = dTransTimeInA;
#if LOG
            Function.objLogWriter.Append(lngAcceptedSocketID, lngSocketTransIDA, 0, 0, "Request data was received.", C_MODULE_NAME);
#endif
            Thread objSpliterThread = new Thread(new ThreadStart(SplitTotalRequest));
            objSpliterThread.Start();
        }
        #endregion

        #region Properties...
        #endregion

        #region Private Methods...
        private void SplitTotalRequest()
        {
            //Total message variables
            string strTotalMessage;
            int intTotalMessageLen = 0;
            int intMessageNumber = 0;

            //Next message variables
            int intNextMessageLen = 0;

            //Others variables
            int intMessageOffSet;

            intMessageOffSet = 0;

/*#if LOG
            Function.objLogWriter.Append(lngSocketID, lngSocketTransID, 0, 0, "Converting total request to Hexadecimal format ...", C_MODULE_NAME);
#endif*/
            strTotalMessage = TranslateRequest(bytTotalRequest, intTotalBytesReceived);
#if LOG
            Function.objLogWriter.Append(lngSocketID, lngSocketTransID, 0, 0, "Total request was converted to Hexadecimal format.", C_MODULE_NAME);
#endif
            //jctb   intTotalMessageLen = strTotalMessage.Length
            //jctb intNextMessageLen = DecOfHex(strTotalMessage.Substring(intMessageOffSet, 4)) * 2

            //jctb
            //jctb If intTotalMessageLen > intNextMessageLen + 4 Then
            //jctb    strMSG &= ". (Multiple Messages)"
            //jctb End If

            try
            {
                do
                {
                    //intMessageNumber += 1
                    //intNextMessageLen = DecOfHex(strTotalMessage.Substring(intMessageOffSet, 4)) * 2

                    //Get Next Message
                    //strNextMessage = strTotalMessage.Substring(intMessageOffSet, intNextMessageLen + 4)

                    //Sending Request to Queue
/*#if LOG
                    Function.objLogWriter.Append(lngSocketID, lngSocketTransID, intMessageNumber, 0, "Sending request to Requests Queue ...", C_MODULE_NAME);
#endif*/
                    Request objRequest = new Request(objSocket,
                                                lngSocketID,
                                                lngSocketTransID,
                                                dTransTimeIn,
                                                intMessageNumber,
                                                strTotalMessage
                                                );

                    Function.objRequestQueue.EnqueueRequest(objRequest);
/*#if LOG
                    Function.objLogWriter.Append(lngSocketID, lngSocketTransID, intMessageNumber, 0, "Request was sent to Requests Queue.", C_MODULE_NAME);
#endif*/
                    intMessageOffSet += intNextMessageLen + 4;

                    while (intMessageOffSet + 3 < intTotalMessageLen && strTotalMessage.Substring(intMessageOffSet, 3) == "000")
                    {
                        intMessageOffSet += 2;
                    }

                    Thread.Sleep(125);
                }
                while (intMessageOffSet + 4 < intTotalMessageLen);
            }
            catch (Exception ex)
            {
#if LOG
                Function.objLogWriter.Append(lngSocketID, lngSocketTransID, intMessageNumber, 0, ex.Message + ". Total:" + strTotalMessage, C_MODULE_NAME);
#endif
            }

        }

        private string TranslateRequest(byte[] bytRequest, int intBytesReceived)
        {
            System.Text.Encoding ascii = System.Text.Encoding.ASCII;
            //TASK 999 RA PLEASE REVIEW THE ASCII.GETCHARS
            char[] chars = ascii.GetChars(bytRequest);

         
            StringBuilder strNewString = new StringBuilder("");
            strNewString.Append(chars);


            //for (int intIndex = 0; intIndex <= intBytesReceived - 1; intIndex++)
            //{
            //    strTempString = bytRequest[intIndex].ToString("000");

            //    if (strTempString == "255")
            //        strNewString += "FF";
            //    else
            //    { 
            //        //jctb  strTempString = Right("0" & Hex(strTempString), 2)
            //      strTempString = Chr(strTempString);

            //        strNewString += strTempString;
            //    }
            //}

            return strNewString.ToString();
        }

        #endregion

        #region Exposed Methods...
        #endregion
    }
}
