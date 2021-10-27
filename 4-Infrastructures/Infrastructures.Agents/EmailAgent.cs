using Core.Entities.Email;
using Interfaces.Agent;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructures.Agents
{
    public class EmailAgent : IEmailAgent
    {
        private const int Timeout = 180000;
        private readonly string _host;
        private readonly int _port;
        private readonly string _user;
        private readonly string _pass;
        private readonly bool _ssl;
        private readonly string _sender;
        private readonly string _isProd;
        public EmailAgent()
        {
            _host = ConfigurationManager.AppSettings["MailServer"];
            _port = int.Parse(ConfigurationManager.AppSettings["Port"]);
            _sender = ConfigurationManager.AppSettings["MailAuthUser"];
            _user = ConfigurationManager.AppSettings["MailAuthUser"];
            _pass = ConfigurationManager.AppSettings["MailAuthPass"];
            _ssl = Convert.ToBoolean(ConfigurationManager.AppSettings["EnableSSL"]);
            _isProd = ConfigurationManager.AppSettings["IsProd"];
        }
        public void Send(EmailSettings setting)
        {
            try
            {
                
                // We do not catch the error here... let it pass direct to the caller
                Attachment att = null;
                var message = new MailMessage() { IsBodyHtml = true };


                if (!string.IsNullOrEmpty(setting.From))
                {
                    var from = new MailAddress(setting.From);
                    message.From = from;
                }
                else 
                {
                    var from = new MailAddress(_sender);
                    message.From = from;
                    setting.From = _sender;
                }
                message.Body = setting.Body;
                message.Subject = setting.Subject;

                if (_isProd != "true")
                {
                   message.CC.Add("alpsadmin@alps.com");
                   message.To.Add("alpsadmin@alps.com");

                    if (setting.RecipientCC != null)
                    {
                       message.CC.Add("alpsadmin@alps.com");
                       message.CC.Add("alpsadmin@alps.com");
                    }
                }
                else
                {
                    if (setting.Recipient != null)
                    {
                        foreach (var recipient in setting.Recipient)
                            message.To.Add(recipient);
                    }
                    if (setting.RecipientCC != null)
                    {
                        foreach (var cc in setting.RecipientCC)
                            message.CC.Add(cc);
                    }
                }
                if (setting.Attachments != null)
                {
                    foreach (var attachment in setting.Attachments)
                        message.Attachments.Add(attachment);
                }
                var smtp = new SmtpClient(_host, _port);


                if (_user.Length > 0 && _pass.Length > 0)
                {
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential(_user, _pass);
                    smtp.EnableSsl = _ssl;
                }

                smtp.Send(message);

                if (att != null)
                    att.Dispose();
                message.Dispose();
                smtp.Dispose();
            }

            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
