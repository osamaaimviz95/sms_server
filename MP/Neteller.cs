using System;
using System.Collections;
using System.Text;
using Common.XML;
using System.Net;
using System.Xml;
using System.Timers;

using Common;

namespace Transaction
{
    class HTTPMessage
    {
        StringBuilder format;
        public HTTPMessage(String format)
        {
            this.format = new StringBuilder(format);
        }

        public string this[string name]
        {
            set
            {
                try
                {
                    format.Replace("{" + name + "}", value);
                }
                catch
                {
                }
            }
        }

        public override string ToString()
        {
            return format.ToString();
        }
    }

    public class Neteller
    {
        private string URI = "https://www.neteller.com";

        public Neteller(string URI)
        {
            this.URI = URI;
        }

        public XMLRecord Withtellerv3(
            string amount,
            string currency,
            string merchant_id,
            string merch_key,
            string merch_pass,
            string net_account
            )
        {
            HTTPMessage message = new HTTPMessage(Constant.WITHTELLERV3);

            message["amount"] = amount;
            message["currency"] = currency;
            message["merchant_id"] = merchant_id;
            message["merch_key"] = merch_key;
            message["merch_pass"] = merch_pass;
            message["net_account"] = net_account;

            XmlTextReader response = new XmlTextReader(URI + message);
            return new XMLRecord(response);
        }

        public XMLRecord Netdirectv4(string amount, 
                                    string merchant_id,
                                    string net_account,
                                    string secure_id,
                                    string merch_transid,
                                    string currency)
        {
            HTTPMessage message = new HTTPMessage(Constant.NETDIRECTV4);
            message["amount"] = amount;
            message["merchant_id"] = merchant_id;
            message["net_account"] = net_account;
            message["secure_id"] = secure_id;
            message["merch_transid"] = merch_transid;
            message["currency"] = currency;

           XmlTextReader response = new XmlTextReader(URI + message);

           return new XMLRecord(response);
        }
    }

    //Request strings
    class Constant
    {
        public static string WITHTELLERV3 =
@"/gateway/withtellerv3.cfm?amount={amount}&currency={currency}&merchant_id={merchant_id}&merch_key={merch_key}&merch_pass={merch_pass}&net_account={net_account}";

        public static string NETDIRECTV4 =
@"/gateway/netdirectv4.cfm?amount={amount}&merchant_id={merchant_id}&net_account={net_account}&secure_id={secure_id}&merch_transid={merch_transid}&currency={currency}";
    }
}
