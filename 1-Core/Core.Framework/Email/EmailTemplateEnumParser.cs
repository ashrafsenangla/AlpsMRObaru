using Core.Entities.Email;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Framework.Email
{
    public static class EmailTemplateEnumParser
    {
        public static string GetFileName(EmailTemplateEnum templateEnum)
        {
            switch (templateEnum)
            {
                case EmailTemplateEnum.NotifyMSRHasBeenFinished:
                    return "NotifyMSRHasBeenFinished.cshtml";
                case EmailTemplateEnum.NotifyMSRHasBeenRejected:
                    return "NotifyMSRHasBeenRejected.cshtml";
                case EmailTemplateEnum.NotifyMSRHasBeenReturned:
                    return "NotifyMSRHasBeenReturned.cshtml";
                case EmailTemplateEnum.NotifyNewMSRRequestHasBeenReceived:
                    return "NotifyNewMSRRequestHasBeenReceived.cshtml";
                case EmailTemplateEnum.NotifyRequesterNewMSRHasBeenSent:
                    return "NotifyRequesterNewMSRHasBeenSent.cshtml";


                case EmailTemplateEnum.NotifyCapexHasBeenFinished:
                    return "NotifyCapexHasBeenFinished.cshtml";
                case EmailTemplateEnum.NotifyCapexHasBeenRejected:
                    return "NotifyCapexHasBeenRejected.cshtml";
                case EmailTemplateEnum.NotifyCapexHasBeenReturned:
                    return "NotifyCapexHasBeenReturned.cshtml";
                case EmailTemplateEnum.NotifyNewCapexRequestHasBeenReceived:
                    return "NotifyNewCapexRequestHasBeenReceived.cshtml";
                case EmailTemplateEnum.NotifyCapexRequestHasBeenReceivedForBuyer:
                    return "NotifyCapexRequestHasBeenReceivedForBuyer.cshtml";

                case EmailTemplateEnum.NotifyRequesterNewPRHasBeenSent:
                    return "NotifyRequesterNewPRHasBeenSent.cshtml";
                case EmailTemplateEnum.NotifyPRHasBeenFinished:
                    return "NotifyPRHasBeenFinished.cshtml";
                case EmailTemplateEnum.NotifyPRHasBeenRejected:
                    return "NotifyPRHasBeenRejected.cshtml";
                case EmailTemplateEnum.NotifyPRHasBeenReturned:
                    return "NotifyPRHasBeenReturned.cshtml";
                case EmailTemplateEnum.NotifyNewPRRequestHasBeenReceived:
                    return "NotifyNewPRRequestHasBeenReceived.cshtml";
                case EmailTemplateEnum.NotifyPRHasBeenFinishedToRequester:
                    return "NotifyPRHasBeenFinishedToRequester.cshtml";
                case EmailTemplateEnum.NotifyNewInvoiceTrackingRequestHasBeenReceived:
                    return "NotifyNewInvoiceTrackingRequestHasBeenReceived.cshtml";
                case EmailTemplateEnum.NotifyRequesterNewInvoiceHasBeenSent:
                    return "NotifyRequesterNewInvoiceHasBeenSent.cshtml";
                case EmailTemplateEnum.NotifyInvoiceHasBeenRejected:
                    return "NotifyInvoiceHasBeenRejected.cshtml";
                case EmailTemplateEnum.NotifyInvoiceHasBeenReturned:
                    return "NotifyInvoiceHasBeenReturned.cshtml";
                case EmailTemplateEnum.NotifyInvoiceHasBeenFinished:
                    return "NotifyInvoiceHasBeenFinished.cshtml";
                case EmailTemplateEnum.NotifyInvoiceOpenPOHasBeenReceived:
                    return "NotifyInvoiceOpenPOHasBeenReceived.cshtml";
                case EmailTemplateEnum.NotifyInvoicePOReviseHasBeenReceived:
                    return "NotifyInvoicePOReviseHasBeenReceived.cshtml";
                case EmailTemplateEnum.NotifyInvoiceNoReferenceHasBeenReceived:
                    return "NotifyInvoiceNoReferenceHasBeenReceived.cshtml";
                    
                default:
                    return "";
            }
        }
    }
}
