using System;
using System.Collections.Generic;
using System.Text;
using Common.XML;
using Common;

namespace Common
{
    class Util
    {
        static XMLList errors = Config.List("errors");

        public static void LoadErrors()
        {
            errors = Config.List("errors");
        }

        public static string ErrorMessage(string id, params object[] arg)
        {
            string message =  String.Format(errors[id], arg);
            return message == "" ? String.Format(Config.DEFAULT_MESSAGE_ERROR, DateTime.Now.ToString("yyMMddhhmmss")) : message;
        }

    }
}
