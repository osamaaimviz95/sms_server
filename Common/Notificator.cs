using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Mail;

using Common;

namespace Common
{
    public class Notificator
    {
        private const string C_MODULE_NAME = "Notificator"; 

        public static void Send(string message)
        {
            LogWriter objLogWriter = new LogWriter();

            try
            {
#if LOG
                objLogWriter.Append("Sending email: " + message, C_MODULE_NAME);
#endif
                SmtpClient smtp = new SmtpClient();
                smtp.Host = Config.Value("smtp.server");
                MailMessage mail = new MailMessage(Config.Value("email.from"),
                    Config.Value("email.to"), Config.Value("email.subject"), message);

                smtp.SendAsync(mail, null);
            }
            catch (Exception ex)
            {
                objLogWriter.Append("Email not sent: " + message + " Error: " + ex.Message, C_MODULE_NAME);
            }
        }
    }
}
