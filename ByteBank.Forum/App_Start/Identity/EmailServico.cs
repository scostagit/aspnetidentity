using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;

namespace ByteBank.Forum.App_Start.Identity
{
    public class EmailServico : IIdentityMessageService
    {
        /*
         * companies which you can use to send emails in high scale.
         * https://www.mailgun.com/
         * https://sendgrid.com/
         */

        private readonly string EMAIL_ORIGEM = ConfigurationManager.AppSettings["emailServico:email_remetente"];
        private readonly string EMAIL_SENHA = ConfigurationManager.AppSettings["emailServico:email_senha"];

        public async Task SendAsync(IdentityMessage message)
        {
            using (var mensagemEmail = new MailMessage())
            {
                mensagemEmail.From = new MailAddress(EMAIL_ORIGEM);

                mensagemEmail.Subject = message.Subject;
                mensagemEmail.To.Add(message.Destination);
                mensagemEmail.Body = message.Body;

                //SMTP :  simple mail transport protocol

                using (var smtpClient = new SmtpClient())
                {
                    smtpClient.UseDefaultCredentials = true;
                    smtpClient.Credentials = new NetworkCredential(EMAIL_ORIGEM, EMAIL_SENHA); //cretiditon

                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network; //it will be execute on the network.
                    smtpClient.Host = "smtp.gmail.com";
                    smtpClient.Port = 587;
                    //smtpClient.EnableSsl = true; //the communicate between our application and Google will be encrypted.

                    smtpClient.Timeout = 20_000; //two seconds: in runtine the compile will remove the "_". we can do it to make easy when look at his number.

                    await smtpClient.SendMailAsync(mensagemEmail);
                }

            }
        }
    }
}