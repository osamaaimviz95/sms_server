using System;
using System.Collections;
using System.Text;
using Common.XML;
using System.Xml;
using System.Reflection;
using MetroProcessDispatcher;
using Common;

namespace FunctionDispatcher
{
    public class Response
    {
        public string Data;
        public string Status;

        public Response(string data, string status)
        {
            this.Data = data;
            this.Status = status;
        }
    }

    public class FunctionRequest
    {
        public static string status;
        char[] sep = new char[] { ',' };
        char[] eom = new char[] { '$' };
        FunctionCollection functions;

        private string MY_NAME = "Function Engine";
        private string path = "";

        public FunctionRequest(string path)
        {
            this.path = path;
            LoadFunctionMap();
        }

        public void LoadFunctionMap()
        {
           functions = new FunctionCollection(Config.GetRoot());
        }

        protected string ListFunctions()
        {
            StringBuilder list = new StringBuilder("");
            foreach(DictionaryEntry entry in functions)
                list.Append(((Function)entry.Value).Name).Append(" ");

            return list.Remove(list.Length-1,1).ToString();
        }

        protected string LoadMaps()
        {
            status = "LOAD OK";
            try
            {
                Config.Load();
                LoadFunctionMap();
                Util.LoadErrors();
                return status;
            }
            catch (Exception e)
            {
                return status = e.Message;
            }
        }

        public static FunctionRequest Create(string path)
        {
            return new FunctionRequest(path);
        }

        public static string Status
        {
            get
            {
                return status;
            }
        }

        public Response Request(string query)
        {
            //JCTB  02/01/2021 
            query = ",/,LSTRNSV001,12345,050820121212,SESS,66.155.207.1,3051231234,1,000000000000834,10000642,/";

            if (query == "")
                return new Response(Util.ErrorMessage("EMPTY QUERY", Config.Value("del"), "0", "0", "90", DateTime.Now.ToString("yyMMddhhmmss")), Config.ERROR_90);

            sep = new char[] { query[0] };
            eom = new char[] { query[1] };

            if (sep[0] == eom[0])
                return new Response(Util.ErrorMessage("END OF MESSAGE EQUAL TO FIELD SEPARATOR", Config.Value("del"), "0", "0", "90", DateTime.Now.ToString("yyMMddhhmmss")), Config.ERROR_90);

            string[] fields = query.Split(sep);

            if (eom[0].ToString() != fields[fields.Length - 1])
                return new Response(Util.ErrorMessage("MISSING EoM", Config.Value("del"), "0", "0", "90", DateTime.Now.ToString("yyMMddhhmmss")), Config.ERROR_90);

            return Process("", query.Substring(3));
        }

        public Response Process(string id, string request)
        {
            int headerLength = Config.IntValue("header.length");
            string[] param = request.Split(sep, headerLength + 1);

            if (functions[param[0]] == null)
                return new Response(Util.ErrorMessage("UNKNOWN FUNCTION", sep[0], param.Length > 2 ? param[1] : "0", "0", Config.ERROR_90, DateTime.Now.ToString("yyMMddhhmmss"), param[0]), Config.ERROR_90);

            //Validate header length
            if (param.Length < headerLength)
                return new Response(Util.ErrorMessage("HEADER EXPECTED", sep[0], param.Length > 2 ? param[1] : "0", "0", Config.ERROR_90, DateTime.Now.ToString("yyMMddhhmmss")), Config.ERROR_90);

            //Validate argument list
            if (param.Length < headerLength + 1)
                return new Response(Util.ErrorMessage("MISSING ARGUMENTS", sep[0], param.Length > 2 ? param[1] : "0", "0", Config.ERROR_90, DateTime.Now.ToString("yyMMddhhmmss")), Config.ERROR_90);

            Hashtable header = new Hashtable();
            header["Transaction Id"] = param[0];
            header["Transaction Num"] = param[1];
            header["Timestamp"] = param[2];
            header["Session Id"] = param[3];
            header["Ip Address"] = param[4];

            switch (param[0])
            {
                case "LSTRNSV001": return new Response(ListFunctions(), Config.ERROR_01);
                case "LODXMLV001": return new Response(LoadMaps(), Config.ERROR_01);
                default: return functions[param[0]][header,sep[0],param[headerLength].Split(sep,StringSplitOptions.None)];
            }
        }

         public string Name
        {
            get
            {
                return MY_NAME;
            }
        }
     }

    //Function
    class Function
    {
        static private int[] range = new int[]
                {
                    5,20, //year
                    1,12,  //month
                    1,31,  //day
                    0,23,  //hour
                    0,59,  //minite
                    0,59  //second
                };
        private string[] descriptor;
        private MethodInfo method;
        private object invoker;
        private string format;
        private char[] _sep;
        private char[] _eom;
 
        const int MAX_EXP_YEAR = 3000;

        public Function(string descriptor, char[] sep, string format, object 
invoker)
        {
            this.descriptor = descriptor.Split(sep);
            this.format = format;
            this.invoker = invoker;
            method = invoker.GetType().GetMethod(this.descriptor[0]);

            //_sep = sep;
            //_eof = eom;
        }

        public string Name
        {
            get
            {
                return descriptor[0];
            }
        }

        protected string ParseTime(string data)
        {
            if (data.Length != range.Length)
                throw new Exception();

            for (int i = 0; i < range.Length / 2; i++)
            {
                try
                {
                    int digit = Int32.Parse(data.Substring(2 * i, 2));
                    if (digit < range[2 * i] || digit > range[2 * i + 1])
                        throw new Exception();
                }
                catch
                {
                    throw new Exception();
                }
            }

            return data;
        }

        protected string ParseXDate(string data)
        {
            if (data.Length != 6)
                throw new Exception("WRONG EXP DATE FORMAT");

            try
            {
                int year = Int32.Parse(data.Substring(0, 4));
                int month = Int32.Parse(data.Substring(4, 2));

                if (year < DateTime.Now.Year || year > MAX_EXP_YEAR)
                    throw new Exception("WRONG EXP DATE FORMAT");

                if (year == DateTime.Now.Year && month < DateTime.Now.Month || month > 12)
                    throw new Exception("WRONG EXP DATE FORMAT");
            }
            catch
            {
                throw new Exception("WRONG EXP DATE FORMAT");
            }

            return data;
        }

        protected string Format(object data, Hashtable header, char respDel)
        {
            Hashtable table = (Hashtable)data;
            StringBuilder result = new StringBuilder(format);

            //timeststamp
            table["Timestamp"] = DateTime.Now.ToString("yyMMddhhmmss");

            //Original TransactionNum
            table["Original Transaction Num"] = header["Transaction Num"];

            //formatting
            ICollection keys = table.Keys;
            foreach (object key in keys)
                result.Replace("{" + key + "}", table[key] + "");

            try
            {
                return String.Format(result.ToString(), respDel);
            }
            catch(Exception e)
            {
                return Util.ErrorMessage("PROBLEM WITH FORMAT", respDel, header["Transaction Num"], header["Response Transaction Id"], "90", DateTime.Now.ToString("yyMMddhhmmss"), e.Message);
            }
        }

        public Response this[Hashtable header, char respDel, params string[] args]
        {
            get
            {
                if (descriptor.Length - header.Count > args.Length)
                    return new Response(Util.ErrorMessage("TOO FEW ARGUMENTS", respDel, header["Transaction Num"], "0", "90", DateTime.Now.ToString("yyMMddhhmmss")),Config.ERROR_90);

                if (descriptor.Length - header.Count < args.Length)
                    return new Response(Util.ErrorMessage("TOO MANY ARGUMENTS", respDel, header["Transaction Num"], "0", "90", DateTime.Now.ToString("yyMMddhhmmss")),Config.ERROR_90);

                try
                {
                    ParseTime(header["Timestamp"].ToString());
                }
                catch
                {
                    return new Response(Util.ErrorMessage("WRONG TIME FORMAT", respDel, header["Transaction Num"], "0", Config.ERROR_90, DateTime.Now.ToString("yyMMddhhmmss"), header["Timestamp"]), Config.ERROR_90);
                }

                try
                {
                    Int64.Parse(header["Transaction Num"].ToString());
                }
                catch
                {
                    return new Response(Util.ErrorMessage("WRONG TRANSACTION NUMBER FORMAT ON HEADER", respDel, header["Transaction Num"], "0", Config.ERROR_90, DateTime.Now.ToString("yyMMddhhmmss"), header["Transaction Num"]), Config.ERROR_90);
                }

                object[] param = new object[args.Length + 1];
                param[0] = header;
                for (int i = 0, j = header.Count; i < args.Length; i++, j++)
                {
                    switch (descriptor[j])
                    {
                        case "n": { try { param[i + 1] = Int64.Parse(args[i]);
                    }
                    catch { return new Response(Util.ErrorMessage("ERROR ON ARG", respDel, header["Transaction Num"], "0", Config.ERROR_90, DateTime.Now.ToString("yyMMddhhmmss"), i), Config.ERROR_90); }; break;
                }
                        case "t": { try { param[i + 1] = ParseTime(args[i]);
                    }
                    catch { return new Response(Util.ErrorMessage("ERROR ON ARG", respDel, header["Transaction Num"], "0", Config.ERROR_90, DateTime.Now.ToString("yyMMddhhmmss"), i), Config.ERROR_90); }; break;
                }
                        case "xd": { try { param[i + 1] =
ParseXDate(args[i]);
                    }
                    catch { return new Response(Util.ErrorMessage("ERROR ON ARG", respDel, header["Transaction Num"], "0", Config.ERROR_90, DateTime.Now.ToString("yyMMddhhmmss"), i), Config.ERROR_90); }; break;
                }
                        default: param[i + 1] = args[i]; break;
                    }
                }

                //invoke the function MP JCTB
                Hashtable invResp = (Hashtable)method.Invoke(invoker, param);
                return new Response(Format(invResp,header,respDel), invResp["Response status"] + "");
            }
        }
    }

    //FunctionCollection
    class FunctionCollection : Hashtable
    {
        MetroProcess invoker;
        public FunctionCollection(XMLList config)
        {
            char[] del = new char[] { config["del"][0] };

            invoker = new MetroProcess(config);

            XMLList prototype = new XMLList("<list>" + 
config.GetXML("prototype") + "</list>");
            XMLList response = new XMLList("<list>" + 
config.GetXML("response") + "</list>");
            foreach (XmlNode item in prototype)
            {
                string id = item.Attributes["id"].Value;
                this[id] = new Function(item.InnerText, del, response[id], 
invoker);
            }
        }

        public Function this[string name]
        {
            get
            {
                return (Function)base[name];
            }
            set
            {
                base[name] = value;
            }
        }
    }
}

