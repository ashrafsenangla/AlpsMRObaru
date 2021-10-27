using RazorEngine.Templating;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Framework.Email
{
    public class RazorParser
    {
        public string Parse(object param, string fullFileDir)
        {
            var templateService = new TemplateService();
            var emailHtmlBody = templateService.Parse(File.ReadAllText(fullFileDir), param, null, fullFileDir);
            return emailHtmlBody;
        }
    }
}
