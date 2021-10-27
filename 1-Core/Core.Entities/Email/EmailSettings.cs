using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.Email
{
    public class EmailSettings
    {
        public string[] Recipient { get; set; }
        public string[] RecipientCC { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public ICollection<Attachment> Attachments { get; set; }
        public string From { get; set; }
        public EmailTemplateEnum TemplateName { get; set; }
    }
}
