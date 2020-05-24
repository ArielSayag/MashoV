using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Mail;
using System.Web.Security;

namespace mashovFinal.Utilities
{
    public class MailHelper
    {
        static readonly string smtpAddr = "smtp.gmail.com";
        static readonly int portNumber = 587;
        static readonly bool enableSSL = true;
        static readonly string emailFromAddress = "mashov.website@gmail.com";   
        static readonly string password = "aA1234Aa";

        public static void SendEMail(string to, string subject, string body)
        {
            using (MailMessage mail = new MailMessage())
            {
                mail.From = new MailAddress(emailFromAddress);
                mail.To.Add(to);
                
                mail.Subject = subject;
                mail.Body = body;
                mail.IsBodyHtml = true;
                
                using (SmtpClient smtp = new SmtpClient(smtpAddr, portNumber))
                {
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential(emailFromAddress, password);
                    smtp.EnableSsl = enableSSL;
                  
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;                    
                    try
                    {                        
                        smtp.Send(mail);
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
            }
        }
    }
}