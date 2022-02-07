using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using FunctionDispatcher;

using Common;

namespace RequestListener
{
    internal sealed class Function
    {
        public static Listener objListener;
        public static LogWriter objLogWriter = new LogWriter();
        public static TransQueue objTransQueue = new TransQueue();
        public static RequestQueue objRequestQueue = new RequestQueue();
        public static AutoResetEvent objWorkingThreadsEvHdl = new AutoResetEvent(false);
        public static ThreadController objThreadController = new ThreadController();
        public static FunctionRequest FunctionDispatcher = FunctionRequest.Create(AppDomain.CurrentDomain.BaseDirectory + "config.xml");
    }
}
