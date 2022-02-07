using System;
using System.Collections.Generic;
using System.Text;
using Common.XML;
using System.Xml;

namespace Common
{
    class Config
    {
        private static XMLList config;
        public static string DEFAULT_MESSAGE_ERROR = "0,0,{0},[MESSAGE ERROR MISSING]";

        public static string ERROR_01 = "1";
        public static string ERROR_90 = "90";

        public static string Status;

        public static string Check()
        {
            string[] entry = {
                "db","header.length","Console.MaxRows",
                "port","netellerURI","del","prototype",
                "response","description","errors"};

            Status = "CONFIG ERROR";
            foreach(string key in entry)
                if (Node(key)==null)
                    return Status += ": '" + key + "' is missing";

            return Status = "OK";
        }

        public static string Load()
        {
            config = XMLObject.LoadList("config.xml");
            return Check();
        }

        public static XmlNode Node(string name)
        {
            return config.GetNode(name);
        }

        public static XMLList GetRoot()
        {
            return config;
        }

        public static string Value(string name)
        {
                return config[name];
        }

        public static int IntValue(string name)
        {
            return Int32.Parse(config[name]);
        }

        public static long LongValue(string name)
        {
            return long.Parse(config[name]);
        }

        public static XMLList List(string name)
        {
            return new XMLList(config.GetNode(name));
        }

        public static XMLRecord Record(string name)
        {
            return new XMLRecord(config.GetNode(name));
        }
    }
}
