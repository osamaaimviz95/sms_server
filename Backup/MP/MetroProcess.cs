using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using e24TranPipeLib;
using Common;
using MetroProcessDispatcher.Internal;
using System.Collections.Specialized;
using System.Net;
using System.IO;
using System.Xml;
using Transaction;
using Common.XML;
using RequestListener;


// pending include the log. 
namespace MetroProcessDispatcher
{

    #region Message Structs...
    public struct Header
    {
        public double Transaction_Id;
        public int status;
    }

    public struct response
    {
        public Header res_header;
        public int pe_trid;
        public int ph_trid;
        public int ph_account;
        public double ph_balance1;
        public double ph_balance2;
        public double ph_balance3;
        public string ph_status;
    }

    public struct responseinc
    {
        public int pe_trid;
        public int ph_trid;
        public int ph_account;
        public double ph_balance1;
        public double ph_balance2;
        public double ph_balance3;
        public string ph_status;
    }

    public struct response_STAVERV001
    {
        public int Status;
        public string Time_Stamp;
        public string Comments;
    }

    public struct response_rahaxi
    {
        public string varResult, varAuth, varAVR, varRef, varTransId, varDate, varErrorMsg;
    }
#endregion

    class MetroProcess
    {
        #region Fields...
        string _dbconn;
        Neteller neteller;

        private const string C_DB_FAIL = "DB Error: ";

        #endregion

        #region Constructors...
        public MetroProcess(XMLList config)
            : base()
        {
            _dbconn = config["db"];
            neteller = new Neteller(config["netellerURI"]);
        }
        #endregion

        #region Private Methods...
        private response_rahaxi SendRahaxi(string card, string cvv2, string expyear,
            string expmonth, string expday, string action, string zip, string address, 
            string member, string amt, string currency, string trackid, string transId)
        {
            try
            {
                e24TranPipeCtl MyObj = new e24TranPipeCtl();

                MyObj.Card = card;
                // purchase
                if (action == "1")
                {
                    MyObj.Zip = zip;
                    MyObj.Addr = address;

                }
                // void purchase            
                if (action == "3")
                {
                    MyObj.TransId = transId;
                }
                // all transactions.           
                MyObj.CVV2 = cvv2;
                MyObj.ExpYear = expyear;
                MyObj.ExpMonth = expmonth;
                MyObj.ExpDay = expday;
                MyObj.Action = action;
                MyObj.Member = member;
                double amount;
                amount = double.Parse(amt) / 100;
                MyObj.Amt = amount.ToString();
                MyObj.Currency = currency;
                MyObj.TrackId = trackid;
                MyObj.Alias = "metro";
                MyObj.ResourcePath = @"C:\3000\";
                MyObj.Udf1 = "UserDefined Field 1";
                MyObj.Udf2 = "UserDefined Field 2";
                MyObj.Udf3 = "UserDefined Field 3";
                MyObj.Udf4 = "UserDefined Field 4";
                MyObj.Udf5 = "UserDefined Field 5";
                MyObj.Timeout = int.Parse(Config.Value("rahaxi.timeout"));


                response_rahaxi response = new response_rahaxi();

                short TransVal = MyObj.PerformTransaction(); //returns 0 for succes -1 for failure

                if (MyObj.Result.Trim() == "")
                    throw new Exception("Result was empty");

                response.varResult = MyObj.Result;
                response.varAuth = MyObj.Auth;
                response.varAVR = MyObj.Avr;
                response.varRef = MyObj.Ref;
                response.varTransId = MyObj.TransId;
                response.varDate = MyObj.Date;
                response.varErrorMsg = MyObj.ErrorMsg;

                return response;
            }
            catch(Exception ex)
            {
                Notificator.Send("Unexpected error at SendRahaxi method: " + ex.Message);

                response_rahaxi response = new response_rahaxi();

                response.varResult = "Unexpected error";
                response.varAuth = "";
                response.varAVR = "";
                response.varRef = "";
                response.varTransId = "";
                response.varDate = "";
                response.varErrorMsg = ex.Message;

                return response;
            }
        }
        
        
#endregion

        #region Exposed Methods...
      
        //**********************************************

        public Hashtable STAVERV001(
            Hashtable header,
            string Option_req,
            string eom)
        {
            Hashtable response = new Hashtable();

            using (Database db = new Database(_dbconn))
            {
                try
                {
                    db.BeginTransaction();

                    ParameterCollection prms = new ParameterCollection();

                    prms.Clear();
//                    prms.Add("?Option_req", Option_req);

                    db.ExecuteNonQuery("Insert into  log_metroprocess (message) values ('STAVERV001') ", prms);
                    prms.Clear();

                    int id = db.GetInt(db.ExecuteScalar("SELECT @@IDENTITY", null));


                    
                    
                    response.Add("Response Transaction Id", id);
                    response.Add("Response status", 1);
                    response.Add("Comments", "OK");
                    response.Add("EOM", eom);

                    db.CommitTransaction();

                    return response;
                }
                catch (Exception ex)
                {
                    db.RollbackTransaction();

                    Notificator.Send(C_DB_FAIL + ex.Message);

                    response.Clear();
                    response.Add("Response Transaction Id", 0);
                    response.Add("Response status", 999);
                    response.Add("Comments", ex.Message);
                    response.Add("EOM", eom);

                    return response;
                }
            }
        }

        public Hashtable BALINQV001(
           Hashtable header,
           string pe_account,
           string moneytype,
           string pe_merchant,
           long pe_terminal)
        {
            Hashtable response = new Hashtable();

            using (Database db = new Database(_dbconn))
            {
                try
                {
                    db.BeginTransaction();


                    ParameterCollection prms = new ParameterCollection();


                    prms.Clear();
                 /////////   prms.Add("?Option_req", "BALINQV001");
                 ////////   db.ExecuteNonQuery("Insert into  log_metroprocess (message) values (?Option_req)", prms);
                    db.ExecuteNonQuery("Insert into  log_metroprocess (message) values ('BALINQV001') ", prms);


                    //#if LOG
                    //                    Function.objLogWriter.Append("Insert...", "MP");
                    //#endif                    
                    prms.Clear();
                       int id = db.GetInt(db.ExecuteScalar("SELECT @@IDENTITY", null));
                       response.Add("Response Transaction Id", id);

                    prms.Clear();
                    ////prms.Add("?pe_account", pe_account);
                    //   prms.Add("?bal_moneytype", moneytype);
                    string strConcat = string.Concat("SELECT phone_balance FROM UEPS_Phone WHERE phone_number = '", pe_account, "'");
                    System.Data.SqlClient.SqlDataReader reader = db.ExecuteReader(strConcat, prms);


                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            response.Add("Response status", 1);
                            response.Add("Available Balance", db.GetLong(reader[0]));
                            response.Add("Comments", "OK");

                            //
                     
                            //#if LOG


                            //                            Function.objLogWriter.Append("Data added", "MP");
                            //#endif                    
                        }
         
         
         
                    }
                    else
                    {
                        response.Add("Response status", 9);
                        response.Add("Available Balance", 0);
                        response.Add("Comments", "Account Does not exist");
                    }
                    reader.Close();

                    strConcat = string.Concat("insert into PosRequestLogNew (ReturnError, ErrorMessage, Product_Hyp_fk, mer_MID, terminal_id, amount) values (", "0, 'BALINQV001', '999056-000', '", pe_merchant, "', ", pe_terminal, ", 0)");
                    db.ExecuteNonQuery(strConcat, prms);

                    
                    db.CommitTransaction();
                    //#if LOG
                    //                    Function.objLogWriter.Append("COMMIT", "MP");
                    //#endif                    


                }
                catch (Exception ex)
                {
                    db.RollbackTransaction();

                    Notificator.Send(C_DB_FAIL + ex.Message);

                    //#if LOG
                    //                    Function.objLogWriter.Append("ROLLBACK", "MP");
                    //#endif                    
                    response.Clear();
                    response.Add("Response Transaction Id", 0);
                    response.Add("Response status", 999);
                    response.Add("Available Balance", 0);
                    response.Add("Comments", ex.Message);
                }
            }

            //#if LOG
            //            Function.objLogWriter.Append("BALINQV001 ENDS", "MP");
            //#endif                    
            return response;
        }

        public Hashtable USECNFV001(
            Hashtable header,
            string pe_account,
            string pe_pin2,
            string pe_Authorization,
            string eom)
        {
            Hashtable response = new Hashtable();
            long auth_amount = 0;
            int auth_id = 0;
            String auth_responseNumber = ""; 
            using (Database db = new Database(_dbconn))
            {
                try
                {
                    db.BeginTransaction();

                    ParameterCollection prms = new ParameterCollection();

                    prms.Clear();
                 //   prms.Add("?Option_req", "BUYUEPV001");
                    db.ExecuteNonQuery("Insert into  log_metroprocess (message) values ('USECNFV001')", prms);

                    prms.Clear();
                    int id = db.GetInt(db.ExecuteScalar("SELECT @@IDENTITY", null));
                    response.Add("Response Transaction Id", id);

                    // increase the account balance.

                    prms.Clear();
        // verify account pin 2
                    // 
                    String  PIN2 = "";
                    prms.Clear();
                    string strConcat = string.Concat("SELECT phone_pin2 FROM UEPS_Phone WHERE phone_number = '", pe_account, "'");
                    System.Data.SqlClient.SqlDataReader reader = db.ExecuteReader(strConcat, prms);


                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            PIN2 = db.GetString(reader[0]);
                        }
                        reader.Close();
                    }
                    else
                    {
                        response.Add("Response status", 9);
                        response.Add("Authorization", 0);
                        response.Add("Merchant", 0);
                        response.Add("Comments", "Account Does not exist");
                        response.Add("EOM", eom);
                        reader.Close();
                        db.CommitTransaction();
                        return response;
                    }// end if has rows
                    reader.Close();

                    if (PIN2 !=  pe_pin2)
                    {
                        response.Add("Response status", 9);
                        response.Add("Authorization", 0);
                        response.Add("Merchant", 0);
                        response.Add("Comments", "Financial PIN invalid");
                        response.Add("EOM", eom);
                        reader.Close();
                        db.CommitTransaction();
                        return response;

                    }

                    // eof validates pin2 

                    prms.Clear();
                    //auth_responseNumber
                   
                    //strConcat = string.Concat("select auth_amount, auth_id, auth_responseNumber from UEPS_AUTH where auth_phone_number = '", pe_account, "' and  auth_session_id = '", header["Session Id"], "'and auth_pre_code = '", pe_Authorization, "' and auth_status = 'P'");
                    strConcat = string.Concat("select auth_amount, auth_id, auth_responseNumber from UEPS_AUTH where auth_phone_number = '", pe_account,  "' and auth_status = 'P'");
                    
                    reader = db.ExecuteReader(strConcat, prms);


                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            auth_amount = db.GetLong(reader[0]);
                            auth_id = db.GetInt(reader[1]);
                            auth_responseNumber = db.GetString(reader[2]);
                        }
                        reader.Close();
                    }
                    else
                    {
                        response.Add("Response status", 9);
                        response.Add("Authorization", 0);
                        response.Add("Merchant", 0);
                        response.Add("Comments", "There is no pre-authorization with the provided information");
                        response.Add("EOM", eom);
                        reader.Close();
                        db.CommitTransaction();
                        return response;
                    }// end if has rows
                    reader.Close();

                    // validates pre-authorization 
        



                    // EOF alidates pre-authorization 
                    
                    
                    
                    //  prms.Add("?bal_available", db.SetDouble(pe_amount / 100));
                  //  prms.Add("?pe_account", pe_account);
                    //                        prms.Add("?bal_moneytype", "1");
                    int pe_amount = 0;
    

                    strConcat = string.Concat("UPDATE UEPS_Phone SET phone_balance = phone_balance - ", auth_amount, ", phone_hold = phone_hold - ", auth_amount,  " WHERE phone_number='", pe_account, "'");
                    db.ExecuteNonQuery(strConcat, prms);

                    Random random = new Random();
                    int num = random.Next(1, 9999999);

                    String Authorization = num.ToString("0000000");

                    strConcat = string.Concat("UPDATE UEPS_AUTH SET auth_status = 'U', auth_auth_num = '", Authorization, "'  WHERE auth_id=", auth_id);
                    db.ExecuteNonQuery(strConcat, prms);


                   //strConcat = string.Concat("insert into PosRequestLogNew (ReturnError, ErrorMessage, Product_Hyp_fk, mer_MID, terminal_id, amount) values (", "0, 'BUYUEPV001', '999055-000', '", pe_merchant, "', ", pe_terminal, ", ", pe_amount, ")");
                   //db.ExecuteNonQuery(strConcat, prms);

                                        db.CommitTransaction();
                    response.Add("Response status", 1);
                    response.Add("Authorization", Authorization);
                    response.Add("Merchant", auth_responseNumber);
                    response.Add("EOM", eom);

                    string strmoney;
                    strmoney = "USD";
                    String stramount;
                    stramount = string.Concat(auth_amount);
                    stramount = string.Concat(stramount.Substring(0, stramount.Length - 2), ".", stramount.Substring(stramount.Length - 2));
#if LOG
                    Function.objLogWriter.Append("amount", stramount);
#endif    

                    strConcat = string.Concat("USECNFV001", "Payment by: ", stramount, " ", strmoney, " was athorized for: ", pe_account);
             

                    
                    response.Add("Comments", strConcat);


                    return response;
                }
                catch (Exception ex)
                {
                    db.RollbackTransaction();

                    Notificator.Send(C_DB_FAIL + ex.Message);

                    response.Clear();
                    response.Add("Response Transaction Id", 0);
                    response.Add("Response status", 999);
                    response.Add("Authorization", 0);
                    response.Add("Merchant", 0);
                    response.Add("Comments", ex.Message);
                    response.Add("EOM", eom);
                    return response;
                }
            }
        }
        //a,n,n,a,E
        public Hashtable USEUEPV001(
            Hashtable header,
            string pe_account,
            long pe_amount,
            long pe_merchant,
            string pe_PIN,
            string eom)
        {
            Hashtable response = new Hashtable();

            using (Database db = new Database(_dbconn))
            {
                try
                {
                    db.BeginTransaction();

                    ParameterCollection prms = new ParameterCollection();

                    prms.Clear();
                //    prms.Add("?Option_req", "USEUEPV001");
                    db.ExecuteNonQuery("Insert into  log_metroprocess (message) values ('USEUEPV001')", prms);

                    prms.Clear();
                    int id = db.GetInt(db.ExecuteScalar("SELECT @@IDENTITY", null));
                    response.Add("Response Transaction Id", id);

                    // verify account balance
                    // 
                    long lng_frombalance = 0;
                    prms.Clear();
                    string strConcat = string.Concat("SELECT phone_balance - Phone_hold FROM UEPS_Phone WHERE phone_number = '", pe_account, "' and phone_pin = '", pe_PIN, "'");
                    System.Data.SqlClient.SqlDataReader reader = db.ExecuteReader(strConcat, prms);

 
                   if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            lng_frombalance = db.GetLong(reader[0]);
                        }
                        reader.Close();
                    }
                    else
                    {
                        response.Add("Response status", 9);
                        response.Add("Authorization", 0);
                        response.Add("NameCity", " ");
                        response.Add("Comments", "Account Does not exist or Pin is Invalid");
                        response.Add("EOM", eom);
                        reader.Close();
                        db.CommitTransaction();
                        return response;
                    }// end if has rows
                    reader.Close();

                    if (lng_frombalance < pe_amount)
                    {
                        response.Add("Response status", 9);
                        response.Add("Authorization", 0);
                        response.Add("NameCity", " ");
                        response.Add("Comments", "Account does not have sufficient balance");
                        response.Add("EOM", eom);
                        reader.Close();
                        db.CommitTransaction();
                        return response;

                    }

                    // increase the  the account balance on hold.

                    Random random = new Random();
                    int num = random.Next(1, 9999999);

                    String Authorization = num.ToString("0000000");

                    
                    prms.Clear();
                    strConcat = string.Concat("UPDATE UEPS_Phone SET Phone_hold = Phone_hold + ", pe_amount, " WHERE phone_number='", pe_account, "'");
                    db.ExecuteNonQuery(strConcat, prms);

                    // merchant phone number  auth_responseNumber
                    string auth_responseNumber;
                    string NameCity; 


                    

                    switch (pe_merchant)
                    {
                        case 9999:
                            auth_responseNumber = "7864029030";
                            NameCity = "Please confirm purhase on Hotel Diplomat Stockholm. Reply with Finacial Pin";
                            break;
                        case 9998:
                            auth_responseNumber = "46705621928";
                            NameCity = "Please confirm purhase on Hotel Diplomat Stockholm. Reply with Finacial Pin";
                            break;
                        case 9997:
                            auth_responseNumber = "7862000635";
                            NameCity = "Please confirm purhase on Fridays Miami. Reply with Finacial Pin";
                            break;
                        case 9996:
                            auth_responseNumber = "+358407253465";
                            NameCity = "Please confirm purhase on Fridays Helsinky. Reply with Finacial Pin";
                            NameCity = "Fridays Helsinky";
                            break;
                        case 9995:
                            auth_responseNumber = "+358407253465";
                            NameCity = "Please confirm purhase on Fridays New York. Reply with Finacial Pin";
                            break;
                        case 9994:
                            auth_responseNumber = "+358407253465";
                            NameCity = "Please confirm purhase on Fridays Paris. Reply with Finacial Pin";
                            break;
                        default:
                            auth_responseNumber = "7862000635";
                            NameCity = "Please confirm purhase on Fridays Atenas. Reply with Finacial Pin";
                            break;
                    }

                    //if (pe_merchant == 9999) 
                    //{
                    //    auth_responseNumber = "17864029030";
                    //}
                    //else
                    //{
                    //    auth_responseNumber = "46705621928";
                    //}
                    prms.Clear();
                    strConcat = string.Concat("INSERT INTO UEPS_AUTH(auth_phone_number, auth_session_id, auth_pre_code, auth_amount, auth_invoice, auth_responseNumber) values ('", pe_account, "', '", header["Session Id"], "', '", Authorization, "', ", pe_amount, ", '", pe_merchant, "', '", auth_responseNumber, "')");
                    db.ExecuteNonQuery(strConcat, prms);
                    
                    
                    
                    response.Add("Response status", 1);
                    response.Add("Authorization", Authorization);
                    response.Add("NameCity", NameCity);
                    response.Add("Comments", "USEUEPV001");
                    response.Add("EOM", eom);


                    strConcat = string.Concat("insert into PosRequestLogNew (ReturnError, ErrorMessage, Product_Hyp_fk, mer_MID,  amount) values (", "0, 'USEUEPV001', '999058-000', '", pe_merchant, "', ", pe_amount, ")");
                    db.ExecuteNonQuery(strConcat, prms);

                    db.CommitTransaction();

                    return response;
                }
                catch (Exception ex)
                {
                    db.RollbackTransaction();

                    Notificator.Send(C_DB_FAIL + ex.Message);

                    response.Clear();
                    response.Add("Response Transaction Id", 0);
                    response.Add("Authorization", 0);
                    response.Add("NameCity", " ");
                    response.Add("Response status", 999);
                    response.Add("Comments", ex.Message);
                    response.Add("EOM", eom);

                    return response;
                }
            }
        }
// Transfer
        public Hashtable TFRBALV001(
            Hashtable header,
            string pe_account,
            long pe_amount,
            long pe_AccountTo,
            string pe_PIN,
            string eom)
        {
            Hashtable response = new Hashtable();
            string phone_name = " ";

            using (Database db = new Database(_dbconn))
            {
                try
                {
                    db.BeginTransaction();

                    ParameterCollection prms = new ParameterCollection();

                    prms.Clear();
                    //    prms.Add("?Option_req", "USEUEPV001");
                    db.ExecuteNonQuery("Insert into  log_metroprocess (message) values ('TFRBALV001')", prms);

                    prms.Clear();
                    int id = db.GetInt(db.ExecuteScalar("SELECT @@IDENTITY", null));
                    response.Add("Response Transaction Id", id);

                    // verify account balance
                    // 
                    long lng_frombalance = 0;
                    prms.Clear();
                    string strConcat = string.Concat("SELECT phone_balance - Phone_hold FROM UEPS_Phone WHERE phone_number = '", pe_account, "' and phone_pin = '", pe_PIN, "'");
                    System.Data.SqlClient.SqlDataReader reader = db.ExecuteReader(strConcat, prms);


                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            lng_frombalance = db.GetLong(reader[0]);
                        }
                        reader.Close();
                    }
                    else
                    {
                        response.Add("Response status", 9);
                        response.Add("Authorization", 0);
                        response.Add("NameCity", " ");
                        response.Add("Comments", "Account Does not exist or Pin is Invalid");
                        response.Add("EOM", eom);
                        reader.Close();
                        db.CommitTransaction();
                        return response;
                    }// end if has rows
                    reader.Close();

                    if (lng_frombalance < pe_amount)
                    {
                        response.Add("Response status", 9);
                        response.Add("Authorization", 0);
                        response.Add("NameCity", " ");
                        response.Add("Comments", "Account does not have sufficient balance");
                        response.Add("EOM", eom);
                        reader.Close();
                        db.CommitTransaction();
                        return response;

                    }
                    // find pe_AccountTo account to trasnfer the money
                    prms.Clear();
                    
                    strConcat = string.Concat("SELECT phone_name FROM UEPS_Phone WHERE phone_number = '", pe_AccountTo, "'");
                    System.Data.SqlClient.SqlDataReader reader2 = db.ExecuteReader(strConcat, prms);


                    if (reader2.HasRows)
                    {
                        while (reader2.Read())
                        {
                            phone_name = db.GetString(reader[0]);
                        }
                        reader2.Close();
                    }
                    else
                    {
                        response.Add("Response status", 9);
                        response.Add("Authorization", 0);
                        response.Add("NameCity", " ");
                        response.Add("Comments", "Account to trasnfer money Does not exist");
                        response.Add("EOM", eom);
                        reader.Close();
                        db.CommitTransaction();
                        return response;
                    }// end if has rows
                    reader.Close();




                    // increase the  the account balance on hold.

                    Random random = new Random();
                    int num = random.Next(1, 9999999);

                    String Authorization = num.ToString("0000000");


                    prms.Clear();
                    strConcat = string.Concat("UPDATE UEPS_Phone SET Phone_hold = Phone_hold + ", pe_amount, " WHERE phone_number='", pe_account, "'");
                    db.ExecuteNonQuery(strConcat, prms);

                    // merchant phone number  auth_responseNumber
                    string auth_responseNumber;


                    auth_responseNumber = pe_AccountTo.ToString();
                    
                    prms.Clear();
                    strConcat = string.Concat("INSERT INTO UEPS_AUTH(auth_phone_number, auth_session_id, auth_pre_code, auth_amount, auth_invoice, auth_responseNumber) values ('", pe_account, "', '", header["Session Id"], "', '", Authorization, "', ", pe_amount, ", '", pe_AccountTo, "', '", auth_responseNumber, "')");
                    db.ExecuteNonQuery(strConcat, prms);



                    response.Add("Response status", 1);
                    response.Add("Authorization", Authorization);
                    response.Add("NameCity", phone_name);

                    response.Add("Comments", "OK");
                    response.Add("EOM", eom);


                    strConcat = string.Concat("insert into PosRequestLogNew (ReturnError, ErrorMessage, Product_Hyp_fk, mer_MID,  amount) values (", "0, 'USEUEPV001', '999058-000', '", pe_AccountTo, "', ", pe_amount, ")");
                    db.ExecuteNonQuery(strConcat, prms);

                    db.CommitTransaction();

                    return response;
                }
                catch (Exception ex)
                {
                    db.RollbackTransaction();

                    Notificator.Send(C_DB_FAIL + ex.Message);

                    response.Clear();
                    response.Add("Response Transaction Id", 0);
                    response.Add("Authorization", 0);
                    response.Add("NameCity", " ");
                    response.Add("Response status", 999);
                    response.Add("Comments", ex.Message);
                    response.Add("EOM", eom);

                    return response;
                }
            }
        }

// invoice payment
        public Hashtable PAYINVV001(
    Hashtable header,
    string pe_account,
    long pe_amount,
    String pe_invoice,
    string pe_PIN,
    string eom)
        {
            Hashtable response = new Hashtable();

            using (Database db = new Database(_dbconn))
            {
                try
                {
                    db.BeginTransaction();

                    ParameterCollection prms = new ParameterCollection();

                    prms.Clear();
                    //    prms.Add("?Option_req", "USEUEPV001");
                    db.ExecuteNonQuery("Insert into  log_metroprocess (message) values ('PAYINVV001')", prms);

                    prms.Clear();
                    int id = db.GetInt(db.ExecuteScalar("SELECT @@IDENTITY", null));
                    response.Add("Response Transaction Id", id);

                    // verify account balance
                    // 
                    long lng_frombalance = 0;
                    
                    prms.Clear();
                    string strConcat = string.Concat("SELECT phone_balance - Phone_hold FROM UEPS_Phone WHERE phone_number = '", pe_account, "' and phone_pin = '", pe_PIN, "'");
                    System.Data.SqlClient.SqlDataReader reader = db.ExecuteReader(strConcat, prms);


                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            lng_frombalance = db.GetLong(reader[0]);
                        }
                        reader.Close();
                    }
                    else
                    {
                        response.Add("Response status", 9);
                        response.Add("Authorization", 0);
                        response.Add("Utility", " ");
                        response.Add("Comments", "Account Does not exist or Pin is Invalid");
                        response.Add("EOM", eom);
                        reader.Close();
                        db.CommitTransaction();
                        return response;
                    }// end if has rows
                    reader.Close();

                    if (lng_frombalance < pe_amount)
                    {
                        response.Add("Response status", 9);
                        response.Add("Authorization", 0);
                        response.Add("Utility", " ");
                        response.Add("Comments", "Account does not have sufficient balance");
                        response.Add("EOM", eom);
                        reader.Close();
                        db.CommitTransaction();
                        return response;

                    }

                    // increase the  the account balance on hold.

                    Random random = new Random();
                    int num = random.Next(1, 9999999);

                    String Authorization = num.ToString("0000000");


                    prms.Clear();
                    strConcat = string.Concat("UPDATE UEPS_Phone SET Phone_hold = Phone_hold + ", pe_amount, " WHERE phone_number='", pe_account, "'");
                    db.ExecuteNonQuery(strConcat, prms);


                    // merchant phone number  auth_responseNumber
                    string auth_responseNumber;



                    switch (pe_account)
                    {
                        case "46702567334":
                            auth_responseNumber = "46705621928";
                            break;
                        case "+17864029030":
                            auth_responseNumber = "+17864029030";
                            break;
                        case "+17862000635":
                            auth_responseNumber = "+17862000635";
                            break;
                        default:
                            auth_responseNumber = "+17862000635";
                            break;
                    }

                    //if (pe_account == "46702567334")
                    //{
                    //    auth_responseNumber = "46705621928";
                    //}
                    //else
                    //{
                    //    auth_responseNumber = "17864029030";
                    //}



                    prms.Clear();
                    strConcat = string.Concat("INSERT INTO UEPS_AUTH(auth_phone_number, auth_session_id, auth_pre_code, auth_amount, auth_invoice, auth_responseNumber) values ('", pe_account, "', '", header["Session Id"], "', '", Authorization, "', ", pe_amount, ", '", pe_invoice, "', '", auth_responseNumber, "')");
                    db.ExecuteNonQuery(strConcat, prms);



                    response.Add("Response status", 1);
                    response.Add("Authorization", Authorization);
                    response.Add("Utility", "Electricity Company");
                    response.Add("Comments", "PAYINVV001");
                    response.Add("EOM", eom);


                    strConcat = string.Concat("insert into PosRequestLogNew (ReturnError, ErrorMessage, Product_Hyp_fk, mer_MID,  amount) values (", "0, 'PAYINVV001', '999058-000', '", pe_invoice, "', ", pe_amount, ")");
                    db.ExecuteNonQuery(strConcat, prms);

                    db.CommitTransaction();

////////////////                    // send sms message
////////////////                    MessageViewer.SMSNotify.Service1 servicenotify = new MessageViewer.SMSNotify.Service1();
////////////////                    string notifyResponse = " ";
////////////////#if LOG
////////////////                    Function.objLogWriter.Append("Trying to send e-mail", notifyResponse);
////////////////#endif    
////////////////                    notifyResponse = servicenotify.Notify("localhost", "7868770285@vtext.com", "78687702785@mysmspay.com", "Test", "Purchase by  USD$70.21 on Fridays - Miami Approved");
////////////////#if LOG
////////////////                    Function.objLogWriter.Append("Send E-mail", notifyResponse);
////////////////#endif     
////////////////                    // send sms message


                    return response;
                }
                catch (Exception ex)
                {
                    db.RollbackTransaction();

                    Notificator.Send(C_DB_FAIL + ex.Message);

                    response.Clear();
                    response.Add("Response Transaction Id", 0);
                    response.Add("Authorization", 0);
                    response.Add("Utility", " ");
                    response.Add("Response status", 999);
                    response.Add("Comments", ex.Message);
                    response.Add("EOM", eom);

                    return response;
                }
            }
        }




        public Hashtable TFRUEPV001(
            Hashtable header,
            string pe_from,
            string pe_to,
            long pe_amount,
            string reason,
            string moneytype,
            string pe_merchant,
            long pe_terminal,
            string pe_PIN)
        {
            Hashtable response = new Hashtable();

            // update the second player balance (increase)
            using (Database db = new Database(_dbconn))
            {
                try
                {
                    db.BeginTransaction();

                    ParameterCollection prms = new ParameterCollection();


                    prms.Clear();
                 //   prms.Add("?Option_req", "TFRUEPV001");
                    db.ExecuteNonQuery("Insert into  log_metroprocess (message) values ('TFRUEPV001')", prms);

                    prms.Clear();
                    int id = db.GetInt(db.ExecuteScalar("SELECT @@IDENTITY", null));
                    response.Add("Response Transaction Id", id);
                    // get available balance information from the first player (from)
                    long lng_frombalance = 0;
                    long lng_tobalance = 0;

                    prms.Clear();
                   // prms.Add("?pe_account", pe_from);
                   // prms.Add("?pe_PIN", pe_PIN);

                    string strConcat = string.Concat("SELECT phone_balance FROM UEPS_Phone WHERE phone_number = '", pe_from, "' and phone_pin = '", pe_PIN, "'");
                    System.Data.SqlClient.SqlDataReader reader = db.ExecuteReader(strConcat, prms);
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            lng_frombalance = db.GetLong(reader[0]);
                        }
                        reader.Close();
                    }
                    else
                    {
                        response.Add("Response status", 9);
                        response.Add("Comments", "Account FROM Does not exist or PIN is Invalid");
                        reader.Close();
                        db.CommitTransaction();
                        return response;
                    }// end if has rows
                    reader.Close();

                    if (lng_frombalance < pe_amount)
                    {
                        response.Add("Response status", 9);
                        response.Add("Comments", "Account FROM Does not have sifficient balance to Trasnfer");
                        reader.Close();
                        db.CommitTransaction();
                        return response;

                    }
                    // get information from the second account (to)
                    prms.Clear();
                   // prms.Add("?pe_account", pe_to);
                    strConcat = string.Concat("SELECT phone_balance FROM UEPS_Phone WHERE phone_number = '", pe_to, "'");
                    System.Data.SqlClient.SqlDataReader reader2 = db.ExecuteReader(strConcat, prms);
                    if (reader2.HasRows)
                    {
                        while (reader2.Read())
                        {
                            lng_tobalance = db.GetLong(reader2[0]);
                        }
                        reader2.Close();
                    }
                    else
                    {
                        response.Add("Response status", 9);
                        response.Add("Comments", "Account TO Does not exist");
                        reader2.Close();
                        db.CommitTransaction();
                        return response;
                    }// end if has rows
                    reader2.Close();

                    // update the first player balance (decrease)
                    prms.Clear();
                    //prms.Add("?bal_available", Pe_amount);
                    //prms.Add("?pe_account", pe_from);
                     strConcat = string.Concat("UPDATE UEPS_Phone SET phone_balance = phone_balance - ", pe_amount, " WHERE phone_number='", pe_from, "'");
                    db.ExecuteNonQuery(strConcat, prms);
                    // update the second player balance (increase)
                    prms.Clear();
                    //prms.Add("?bal_available", Pe_amount);
                    //prms.Add("?pe_account", pe_to);
                    strConcat = string.Concat("UPDATE UEPS_Phone SET phone_balance = phone_balance + ", pe_amount, " WHERE phone_number='", pe_to, "'");
                    db.ExecuteNonQuery(strConcat, prms);


                    strConcat = string.Concat("insert into PosRequestLogNew (ReturnError, ErrorMessage, Product_Hyp_fk, mer_MID, terminal_id, amount) values (", "0, 'TFRUEPV001', '999057-000', '", pe_merchant, "', ", pe_terminal, ", ", pe_amount, ")");
                    db.ExecuteNonQuery(strConcat, prms);



                    db.CommitTransaction();
                    response.Add("Response status", 1);
                    response.Add("Comments", "OK");
                    return response;
                }
                catch (Exception ex)
                {
                    db.RollbackTransaction();

                    Notificator.Send(C_DB_FAIL + ex.Message);

                    response.Clear();
                    response.Add("Response Transaction Id", 0);
                    response.Add("Response status", 999);
                    response.Add("Comments", ex.Message);
                    return response;
                }
            }

        }
        //=========================================================================
        
        
        
        //***********************************************
        public Hashtable BALINQV002(
            Hashtable header, 
            long pe_account, 
            string moneytype)
        {
            Hashtable response = new Hashtable();

            using (Database db = new Database(_dbconn))
            {
                try
                {
                    db.BeginTransaction();


                    ParameterCollection prms = new ParameterCollection();
                 
                 

                    prms.Clear();
                    prms.Add("?Option_req", db.SetString("BALINQV001"));
                    db.ExecuteNonQuery("Insert into  log_metroprocess (message) values (?Option_req)", prms);

//#if LOG
//                    Function.objLogWriter.Append("Insert...", "MP");
//#endif                    
                    prms.Clear();
                    int id = db.GetInt(db.ExecuteScalar("SELECT @@IDENTITY", null));
                    response.Add("Response Transaction Id", id);

                    prms.Clear();
                    prms.Add("?pe_account", db.SetLong(pe_account));
                    prms.Add("?bal_moneytype", moneytype);
                    System.Data.SqlClient.SqlDataReader reader = db.ExecuteReader("SELECT * FROM Balance WHERE bal_account_id = ?pe_account  and bal_moneytype = ?bal_moneytype", prms);
                     

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            response.Add("Response status", 1);
                            response.Add("Available Balance", db.GetLong(reader[1]));
                            response.Add("Chips Balance", db.GetLong(reader[2]));
                            response.Add("Total Balance", db.GetLong(reader[1])+ db.GetLong(reader[2]));
                            response.Add("Comments", "OK");
//#if LOG
//                            Function.objLogWriter.Append("Data added", "MP");
//#endif                    
                        }
                    }
                    else
                    {
                        response.Add("Response status", 9);
                        response.Add("Available Balance", 0);
                        response.Add("Chips Balance", 0);
                        response.Add("Total  Balance", 0);
                        response.Add("Comments", "Account Does not exist");
                    }
                    reader.Close();
                    db.CommitTransaction();
//#if LOG
//                    Function.objLogWriter.Append("COMMIT", "MP");
//#endif                    


                }
                catch (Exception ex)
                {
                    db.RollbackTransaction();

                    Notificator.Send(C_DB_FAIL + ex.Message);

//#if LOG
//                    Function.objLogWriter.Append("ROLLBACK", "MP");
//#endif                    
                    response.Clear();
                    response.Add("Response Transaction Id", 0);
                    response.Add("Response status", 999);
                    response.Add("Available Balance", 0);
                    response.Add("Chips Balance", 0);
                    response.Add("Total  Balance", 0);
                    response.Add("Comments", ex.Message);
                }
            }

//#if LOG
//            Function.objLogWriter.Append("BALINQV001 ENDS", "MP");
//#endif                    
            return response;
        }

 
        public Hashtable DEPCRDV001(
            Hashtable header, 
            long pe_account, 
            string pe_card, 
            string pe_cvv2, 
            string pe_expiration_date, 
            string pe_name, 
            long pe_amount, 
            string pe_currency)
        {
            Hashtable response = new Hashtable();

            using (Database db = new Database(_dbconn))
            {
                try
                {
                    db.BeginTransaction();

                    ParameterCollection prms = new ParameterCollection();
                 
                    prms.Clear();
                    prms.Add("?Option_req", "DEPCRDV001");
                    db.ExecuteNonQuery("Insert into  log_metroprocess (message) values (?Option_req)", prms);

                    prms.Clear();  
                    int id = db.GetInt(db.ExecuteScalar("SELECT @@IDENTITY", null));
// execute rahaxi interface to get the money form the credit/debit card

                    string pestr_currency = "840";

                 
                    switch (pe_currency)
                    {
                        case "USD": 
                            pestr_currency = "840";
                            break;
                        case "SEK": 
                            pestr_currency = "752";
                            break;
                        case "CHF": 
                            pestr_currency = "756";
                            break;
                        case "GBP": 
                            pestr_currency = "826";
                            break;
                        case "AUD": 
                            pestr_currency = "036";
                            break;
                        case "EUR": 
                            pestr_currency = "978";
                            break;
                        case "BSD": 
                            pestr_currency = "044";
                            break;
                        case "BMD": 
                            pestr_currency = "060";
                            break;
                        case "BRL": 
                            pestr_currency = "986";
                            break;
                        case "BGN": 
                            pestr_currency = "975";
                            break;
                        case "CAD": 
                            pestr_currency = "124";
                            break;
                        case "KYD": 
                            pestr_currency = "136";
                            break;
                        case "CLP": 
                            pestr_currency = "152";
                            break;
                        case "CNY": 
                            pestr_currency = "156";
                            break;
                        case "COP": 
                            pestr_currency = "170";
                            break;
                        case "DOP": 
                            pestr_currency = "214";
                            break;
                        case "DKK": 
                            pestr_currency = "208";
                            break;
                        case "HKD": 
                            pestr_currency = "344";
                            break;
                        case "JPY": 
                            pestr_currency = "392";
                            break;
                        case "KPW": 
                            pestr_currency = "408";
                            break;
                        case "KRW": 
                            pestr_currency = "410";
                            break;
                        case "MXN": 
                            pestr_currency = "484";
                            break;
                        case "ANG": 
                            pestr_currency = "532";
                            break;
                        case "NZD": 
                            pestr_currency = "554";
                            break;
                        case "PHP": 
                            pestr_currency = "608";
                            break;
                        case "PLN": 
                            pestr_currency = "985";
                            break;
                    }

                    response_rahaxi rahaxi = SendRahaxi(
                        pe_card, 
                        pe_cvv2, 
                        pe_expiration_date.Substring(0, 4), 
                        pe_expiration_date.Substring(4, 2), 
                        "01", "1", " ", " ", 
                        pe_name, 
                        pe_amount.ToString(),
                        pestr_currency, 
                        id.ToString(),
                        " ");
// if result = "CAPTURED" --> we get the money
                   
                    if ( rahaxi.varResult == "CAPTURED")  
                    {
                        // increase the account balance.

                        prms.Clear();
                        prms.Add("?bal_available", db.SetDouble(pe_amount/100));
                        prms.Add("?pe_account", db.SetLong(pe_account));
                        prms.Add("?bal_moneytype", "1");
                        db.ExecuteNonQuery("UPDATE Balance SET bal_available = bal_available + ?bal_available WHERE bal_account_id =  ?pe_account and bal_moneytype = ?bal_moneytype", prms);
                        response.Add("Response status", 1);
                        response.Add("Comments", "OK");
           
                    }
                    else
                    {
                        response.Add("Response status", 8);
                        response.Add("Comments", "There was a problem Processing Transaction with Rahaxi");
                    }
                 
                    response.Add("Response Transaction Id", id);
                    response.Add("Result", rahaxi.varResult);
                    response.Add("Authorization Code", rahaxi.varAuth);
                    response.Add("Reference", rahaxi.varRef);
                    response.Add("Transaction ID", rahaxi.varTransId);
                    response.Add("Error Message", rahaxi.varErrorMsg);
                    db.CommitTransaction();

                    return response;
                }
                catch (Exception ex)
                {
                    db.RollbackTransaction();

                    Notificator.Send(C_DB_FAIL + ex.Message);

                    response.Clear();
                    response.Add("Response Transaction Id", 0);
                    response.Add("Response status", 999);
                    response.Add("Result", "ERROR");
                    response.Add("Authorization Code", " ");
                    response.Add("Reference", " ");
                    response.Add("Transaction ID", " ");
                    response.Add("Error Message", " ");
                    response.Add("Comments", "OK");
                    response.Add("Comments", ex.Message);

                    return response;
                }
            }
        }


        public Hashtable WTHNTAV001(
            Hashtable header,
            long amount,
            string currency,
            string merchant_id,
            string merch_key,
            string merch_pass,
            string net_account
            )
        {
            Hashtable response = new Hashtable();
            Hashtable HT_Neteller = new Hashtable();

            try
            {
                XMLRecord record = neteller.Withtellerv3(
                    amount.ToString(),
                    currency,
                    merchant_id,
                    merch_key,
                    merch_pass,
                    net_account);

                foreach (XmlNode node in record)
                    HT_Neteller[node.Name] = node.InnerText;

                //Escribe desde aqui

                response["Comments"] = "OK";
                return response;
            }
            catch (Exception ex)
            {
                Notificator.Send("Unexpected error at NetTeller WTHNTAV001 method: " + ex.Message);

                response["Comments"] = ex.Message;
                return response;
            }
        }


        public Hashtable DEPNTAV001(
            Hashtable header, 
            long pe_account, 
            long pe_amount,
            long pe_netaccount, 
            long pe_secureid, 
            string pe_currency, 
            string pe_bankacc,
            string pe_merchantid)
        {
            Hashtable response = new Hashtable();

            try
            {
                string transactionId = "0";

                XMLRecord record = neteller.Netdirectv4(
                    pe_amount.ToString(),
                    pe_merchantid,
                    pe_netaccount.ToString(),
                    pe_secureid.ToString(),
                    transactionId,
                    pe_currency);

                foreach (XmlNode node in record)
                    response[node.Name] = node.InnerText;

                //Empieza escribiendo aqui

                return response;
            }
            catch (Exception ex)
            {
                Notificator.Send("Unexpected error at NetTeller DEPNTAV001 method: " + ex.Message);

                response["Comments"] = ex.Message;
                return response;
            }
        }

        public Hashtable UPCHBLV001(
            Hashtable header, 
            long pe_account,
            long pe_amount,
            string pe_type,
            string pe_updbal,
            string pe_reason,
            string pe_moneytype
            )
        {
            Hashtable response = new Hashtable();
            using (Database db = new Database(_dbconn))

            {
                try
                {
                    db.BeginTransaction();

                    ParameterCollection prms = new ParameterCollection();
                  
                    prms.Clear();
                    prms.Add("?Option_req", "UPCHBLV001");
                    db.ExecuteNonQuery("Insert into  log_metroprocess (message) values (?Option_req)", prms);

                    prms.Clear();
                    int id = db.GetInt(db.ExecuteScalar("SELECT @@IDENTITY", null));
                    response.Add("Response Transaction Id", id);
                    if ((pe_type == "I") | (pe_type == "D"))

                    {
                        
                        prms.Clear();
                        prms.Add("?bal_chips", pe_amount / 100);
                        prms.Add("?pe_account", pe_account);
                        prms.Add("?bal_moneytype", pe_moneytype);

                        if (pe_type == "I")
                        {
                            // increase the bal_chips chips balace.
                            if (pe_updbal == "Y")
                            {
                                db.ExecuteNonQuery("UPDATE Balance SET bal_chips = bal_chips + ?bal_chips, bal_available = bal_available - ?bal_chips WHERE bal_account_id =  ?pe_account and bal_moneytype = ?bal_moneytype", prms);
                                response.Add("Response status", 1);
                                response.Add("Comments", "OK");
                            }
                            else
                            {
                                db.ExecuteNonQuery("UPDATE Balance SET bal_chips = bal_chips + ?bal_chips WHERE bal_account_id =  ?pe_account and bal_moneytype = ?bal_moneytype", prms);
                                response.Add("Comments", "OK");
                                response.Add("Response status", 1);
                            }
                            
                            }

                        if (pe_type == "D")
                        {
                            // decrease the bal_chips chips balace.

                            if (pe_updbal == "Y")
                            {
                                db.ExecuteNonQuery("UPDATE Balance SET bal_chips = bal_chips - ?bal_chips, bal_available = bal_available + ?bal_chips WHERE bal_account_id =  ?pe_account and bal_moneytype = ?bal_moneytype", prms);
                                response.Add("Comments", "OK");
                                response.Add("Response status", 1);
                            }
                            else
                            {
                                db.ExecuteNonQuery("UPDATE Balance SET bal_chips = bal_chips - ?bal_chips WHERE bal_account_id =  ?pe_account and bal_moneytype = ?bal_moneytype", prms);
                                response.Add("Comments", "OK");
                                response.Add("Response status", 1);
                            }
                            
                            }

                    }
                    else
                    {
                        response.Add("Response status", 7);
                        response.Add("Comments", "The Type of transaction is invalid");
                    }

                    db.CommitTransaction();

                    return response;
                }
                catch (Exception ex)
                {
                    db.RollbackTransaction();

                    Notificator.Send(C_DB_FAIL + ex.Message);

                    response.Clear();
                    response.Add("Response Transaction Id", 0);
                    response.Add("Response status", 999);
                    response.Add("Comments", ex.Message);

                    return response;
                }
            }
        }

        public Hashtable P2PTFRV001(
            Hashtable header, 
            long pe_from,
            long pe_to,
            long Pe_amount,
            string reason,
            string moneytype)
        {
            Hashtable response = new Hashtable();

// update the second player balance (increase)
            using (Database db = new Database(_dbconn))
            {
                try
                {
                    db.BeginTransaction();

                    ParameterCollection prms = new ParameterCollection();
                   
                    
                    Pe_amount = Pe_amount / 100;

                    prms.Clear();
                    prms.Add("?Option_req", "UPBNBLV001");
                    db.ExecuteNonQuery("Insert into  log_metroprocess (message) values (?Option_req)", prms);

                    prms.Clear();
                    int id = db.GetInt(db.ExecuteScalar("SELECT @@IDENTITY", null));
                    response.Add("Response Transaction Id", id);
                        // get available balance information from the first player (from)
                        long lng_frombalance = 0;
                        long lng_tobalance = 0;

                        prms.Clear();
                        prms.Add("?pe_account", pe_from);
                        prms.Add("?bal_moneytype", moneytype);
                        SqlDataReader reader = db.ExecuteReader("SELECT bal_available FROM Balance WHERE bal_account_id = ?pe_account and bal_moneytype = ?bal_moneytype", prms);
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                              lng_frombalance = db.GetLong(reader[0]);
                            }
                            reader.Close();
                        }
                        else
                        {
                            response.Add("Response status", 9);
                             response.Add("Comments", "Account FROM Does not exist");
                            reader.Close();
                            db.CommitTransaction();
                            return response;
                         }// end if has rows
                        reader.Close();

                        if (lng_frombalance < Pe_amount)
                        {
                            response.Add("Response status", 9);
                            response.Add("Comments", "Account FROM Does not have sifficient balance to Trasnfer");
                            reader.Close();
                            db.CommitTransaction();
                            return response;

                        }

                        // get information from the second player (to)

                        prms.Clear();
                        prms.Add("?pe_account", pe_to);
                        prms.Add("?bal_moneytype", moneytype);
                        reader = db.ExecuteReader("SELECT bal_available FROM Balance WHERE bal_account_id = ?pe_account and bal_moneytype = ?bal_moneytype", prms);
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                              lng_tobalance = db.GetLong(reader[0]);
                            }
                            reader.Close();
                        }
                        else
                        {
                            response.Add("Response status", 9);
                            response.Add("Comments", "Account TO Does not exist");
                            reader.Close();
                            db.CommitTransaction();
                            return response;
                         }// end if has rows
                        reader.Close();
                    
// update the first player balance (decrease)
                        prms.Clear();
                        prms.Add("?bal_available", Pe_amount);
                        prms.Add("?pe_account", pe_from);
                        prms.Add("?bal_moneytype", moneytype);
                        db.ExecuteNonQuery("UPDATE Balance SET bal_available = bal_available - ?bal_available WHERE bal_account_id =  ?pe_account and bal_moneytype = ?bal_moneytype", prms);
                    
// update the second player balance (increase)
                        prms.Clear();
                        prms.Add("?bal_available", Pe_amount);
                        prms.Add("?pe_account", pe_to);
                        prms.Add("?bal_moneytype", moneytype);
                        db.ExecuteNonQuery("UPDATE Balance SET bal_available = bal_available + ?bal_available WHERE bal_account_id =  ?pe_account and bal_moneytype = ?bal_moneytype", prms);

                    db.CommitTransaction();
                    response.Add("Response status", 1);
                    response.Add("Comments", "OK");
                    return response;
                }
                catch (Exception ex)
                {
                    db.RollbackTransaction();

                    Notificator.Send(C_DB_FAIL + ex.Message);

                    response.Clear();
                    response.Add("Response Transaction Id", 0);
                    response.Add("Response status", 999);
                    response.Add("Comments", ex.Message);
                    return response;
                }
            }
        
        }

        public Hashtable NUCUACV001(
            Hashtable header, 
            long accountid, 
            string name, //customer name
            string address1,
            string address2,
            string city,
            string state,
            string ZIP,
            string country,
            string Phone1,
            string email,
            string status       //customer status
            )
        {
            Hashtable response = new Hashtable();
            using (Database db = new Database(_dbconn))
            {
                try
                {
                    db.BeginTransaction();
                    ParameterCollection prms = new ParameterCollection();
               
// create log file
                    prms.Clear();
                    prms.Add("?Option_req", "NUCUACV001");
                    db.ExecuteNonQuery("Insert into  log_metroprocess (message) values (?Option_req)", prms);

                    prms.Clear();
                    int id = db.GetInt(db.ExecuteScalar("SELECT @@IDENTITY", null));
                    response.Add("Response Transaction Id", id);

                    long  newaccount = 0;

                    if (accountid == newaccount)
                    {


                        // create customer record
                        prms.Clear();
                        prms.Add("?Cust_Name", name);
                        db.ExecuteNonQuery("Insert into  Customer (Cust_Name) values (?Cust_Name)", prms);
                        // Get new customer key
                        prms.Clear();
                        int new_cust_key = db.GetInt(db.ExecuteScalar("SELECT @@IDENTITY", null));

                        // create address records
                        prms.Clear();
                        prms.Add("?Addr_city", city);
                        prms.Add("?Addr_Country", country);
                        prms.Add("?Addr_Line1", address1);
                        prms.Add("?Addr_Line2", address2);
                        prms.Add("?Addr_Phone1", Phone1);
                        prms.Add("?Addr_Phone2", email);
                        prms.Add("?Addr_State", state);
                        prms.Add("?Addr_ZipCode", ZIP);
                        prms.Add("?Addr_Cust_id", new_cust_key);
                        db.ExecuteNonQuery("Insert into  Adress (Addr_city, Addr_Country, Addr_Line1, Addr_Line2, Addr_Phone1, Addr_Phone2, Addr_State, Addr_ZipCode, Addr_Cust_id ) values (?Addr_city, ?Addr_Country, ?Addr_Line1, ?Addr_Line2, ?Addr_Phone1, ?Addr_Phone2, ?Addr_State, ?Addr_ZipCode, ?Addr_Cust_id)", prms);
                        // Get new customer key
                        prms.Clear();
                        int new_addr_key = db.GetInt(db.ExecuteScalar("SELECT @@IDENTITY", null));
                        // create balance records real money
                        prms.Clear();
                        prms.Add("?bal_curr_Id", "0");
                        prms.Add("?bal_addr_Id", new_addr_key);
                        prms.Add("?bal_customer_id", new_cust_key);
                        prms.Add("?bal_account_id", new_cust_key);
                        prms.Add("?bal_moneytype", "1");
                        db.ExecuteNonQuery("Insert into  Balance (bal_account_id, bal_curr_Id, bal_addr_Id, bal_customer_id, bal_moneytype ) values (?bal_account_id, ?bal_curr_Id, ?bal_addr_Id, ?bal_customer_id, ?bal_moneytype)", prms);
                        // get the balance record key
                        prms.Clear();

                        response.Add("Customer Account Id", new_cust_key);
                        response.Add("Response status", 1);
                        response.Add("Comments", "OK");
                        // create balance records virtual money
                        prms.Clear();
                        prms.Add("?bal_curr_Id", "0");
                        prms.Add("?bal_addr_Id", new_addr_key);
                        prms.Add("?bal_customer_id", new_cust_key);
                        prms.Add("?bal_account_id", new_cust_key);
                        prms.Add("?bal_moneytype", "0");
                        db.ExecuteNonQuery("Insert into  Balance (bal_account_id, bal_curr_Id, bal_addr_Id, bal_customer_id, bal_moneytype ) values (?bal_account_id, ?bal_curr_Id, ?bal_addr_Id, ?bal_customer_id, ?bal_moneytype)", prms);
                        db.CommitTransaction();
                        return response;
                    }
                    else
                    {
                        response.Add("Customer Account Id", accountid);
                        response.Add("Response status", 1);
                        response.Add("Comments", "OK");
                        return response;


                    }

                }
                catch (Exception ex)
                {
                    db.RollbackTransaction();

                    Notificator.Send(C_DB_FAIL + ex.Message);

                    response.Clear();
                    response.Add("Response Transaction Id", 0);
                    response.Add("Response status", 9);
                    response.Add("Customer Account Id", 0);
                    response.Add("Comments", ex.Message);

                    return response;
                }
            }
        }
        #endregion
    }
}
// 		<item id="DEPNTAV001">DEPNTAV001,12345,050820121212,SESS,IP,n,n,n,n,a</item>
//		<item id="WTHNTAV001">WTHNTAV001,12345,050820121212,SESS,IP,n,n,n,a</item>
//		<item id="DEPNTAV001">{Original Transaction Num}{0}{Response Transaction Id}{0}{Response status}{0}{Timestamp}{0}{Result}{0}{Transaction ID}{0}{Error Message}{0}[{Comments}]</item>
//		<item id="WTHNTAV001">{Original Transaction Num}{0}{Response Transaction Id}{0}{Response status}{0}{Timestamp}{0}{Result}{0}{Transaction ID}{0}{Error Message}{0}[{Comments}]</item>
