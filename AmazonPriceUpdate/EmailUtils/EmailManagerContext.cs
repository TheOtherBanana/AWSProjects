using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace EmailUtils
{
    public class EmailManagerContext
    {
        private string userId;
        private string password;
        private string host;
        private string displayName;
        private SmtpClient smtpClient;

        private static EmailManagerContext instance;

        public static EmailManagerContext Instance
        {
            get
            {
                if(instance == null)
                {
                    throw new ArgumentNullException("EMail context Instance is null");
                }

                return instance;
            }
        }

        private EmailManagerContext(string userid, string password, string host, string displayName)
        {
            this.userId = userid;
            this.password = password;
            this.host = host;
            this.displayName = displayName;

            MailMessage mail = new MailMessage();

            mail.To.Add(this.userId);
            mail.From = new MailAddress(this.userId); ;
            mail.Subject = "Context Initialized";
            mail.SubjectEncoding = Encoding.UTF8;
            mail.Body = "Context Initialized. Time = " + DateTime.UtcNow;
            mail.BodyEncoding = Encoding.UTF8;

            SmtpClient smtpClient = new SmtpClient();
            var credential = new NetworkCredential(this.userId, this.password);
            smtpClient.Port = 587;
            smtpClient.Host = this.host;
            smtpClient.EnableSsl = true;
            smtpClient.UseDefaultCredentials = false;
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            this.smtpClient = smtpClient;
            this.smtpClient.Credentials = credential;
            this.smtpClient.Send(mail);
        }

        public static void Initialize(string userid, string password, string host, string displayName)
        {
            //Trace: Initializing Email context

            Debug.Assert(userid != null, "Username cannot be null");
            Debug.Assert(password != null, "Password cannot be null");
            Debug.Assert(host != null, "Host addresss cannot be null");
            Debug.Assert(displayName != null, "Display Name cannot be null");

            instance = new EmailManagerContext(userid, password, host, displayName);
        }

        public bool SendHtmlEmail(MailMessage mail)
        {
            bool isSent = false;

            mail.From = new MailAddress(this.userId); ;
            
            this.smtpClient.Send(mail);
            isSent = true;

            return isSent;
        }
    }
}
