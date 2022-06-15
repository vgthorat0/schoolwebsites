using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;

namespace Website.Utils
{
    public class EmailService 
    {
        public static Task SendAsync(string Destination,string Subject, string Body)
        {
            SmtpClient client = new SmtpClient();
            client.Host = ConfigurationManager.AppSettings["SmtpHost"];
            client.Port = Convert.ToInt32(ConfigurationManager.AppSettings["SmtpPort"]);
            client.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["SupportEmailAddr"], ConfigurationManager.AppSettings["SupportEmailPass"]);
            
            
            client.EnableSsl = true;

            return client.SendMailAsync(ConfigurationManager.AppSettings["SupportEmailAddr"],
                                        Destination,
                                        Subject,
                                        Body);
        }

    }

}