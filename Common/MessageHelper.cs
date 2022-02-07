using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    public delegate void RLAdminMessageEventHanlder(RLAdminMessageType type);
    
    public enum RLAdminMessageType
    {
        Close = 0
    }

    public class MessageHelper
    {
        public event RLAdminMessageEventHanlder CatchedRLAdminMessage;

        public void SendRLAdminMessage(RLAdminMessageType type)
        {
            CatchedRLAdminMessage(type);
        }
    }
}
