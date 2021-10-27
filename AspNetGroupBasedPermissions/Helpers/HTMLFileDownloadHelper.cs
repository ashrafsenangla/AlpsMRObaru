using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace AspNetGroupBasedPermissions.Helpers
{
    public static class HTMLFileDownloadHelper
    {
        public static MvcHtmlString FileActionLink(this HtmlHelper htmlHelper, string fileName, object htmlAttributes,string linkText="Download File")
        {
            string htmlAttributesString = string.Empty;
            if (htmlAttributes != null)
            {
                RouteValueDictionary d = new RouteValueDictionary(htmlAttributes);
                for (int i = 0; i < d.Keys.Count; i++)
                {
                    htmlAttributesString += " " + d.Keys.ElementAt(i) + "=" + d.Values.ElementAt(i);
                }
            }
            StringBuilder ancor = new StringBuilder();
            ancor.Append("<a ");
            if (htmlAttributesString != string.Empty)
            {
                ancor.Append(htmlAttributesString);
            }
            ancor.Append(" href='");
            ancor.Append("/FileUpload");
            ancor.Append("/" + fileName);
            ancor.Append("'");
            ancor.Append(" title='"+fileName+"' ");
            ancor.Append(" download ");
            ancor.Append(">");
            ancor.Append(linkText);
            ancor.Append("</a>");
            return new MvcHtmlString(ancor.ToString());
        }

        public static string AddDoubleQuotes(this string value)
        {
            return "\"" + value + "\"";
        }

        public static string FileActionUrl(this HtmlHelper htmlHelper, string fileName, object htmlAttributes, string linkText = "Download File")
        {
            return FileActionUrl(fileName, htmlAttributes, linkText);
        }

        private static string FileActionUrl(string fileName, object htmlAttributes, string linkText)
        {
            string queryString = string.Empty;
            string htmlAttributesString = string.Empty;
            string area = string.Empty;
            StringBuilder sb = new StringBuilder();
            if (fileName != string.Empty)
            {
                sb.Append("/FileUpload/" + fileName);
            }
            return sb.ToString();
        }

        internal static string FileActionUrl(string fileName)
        {
            string queryString = string.Empty;
            string htmlAttributesString = string.Empty;
            string area = string.Empty;
            StringBuilder sb = new StringBuilder();
            if (fileName != string.Empty)
            {
                sb.Append("/FileUpload/" + fileName);
            }
            return sb.ToString();
        }
    }
}