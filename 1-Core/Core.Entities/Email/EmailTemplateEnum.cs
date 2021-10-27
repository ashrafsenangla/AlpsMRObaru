using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.Email
{
    public enum EmailTemplateEnum
    {
        NotifyRequesterNewMSRHasBeenSent,
        NotifyNewMSRRequestHasBeenReceived,
        NotifyMSRHasBeenReturned,
        NotifyMSRHasBeenRejected,
        NotifyMSRHasBeenFinished,
        SendRFQToVendor,

        NotifyRequesterNewCapexHasBeenSent,
        NotifyNewCapexRequestHasBeenReceived,
        NotifyCapexHasBeenReturned,
        NotifyCapexHasBeenRejected,
        NotifyCapexHasBeenFinished,
        NotifyCapexRequestHasBeenReceivedForBuyer,

        NotifyRequesterNewPRHasBeenSent,
        NotifyNewPRRequestHasBeenReceived,
        NotifyPRHasBeenReturned,
        NotifyPRHasBeenRejected,
        NotifyPRHasBeenFinished,
        NotifyPRHasBeenFinishedToRequester,
        NotifyQHSESubmission, 
        NotifyNewInvoiceTrackingRequestHasBeenReceived,
        NotifyRequesterNewInvoiceHasBeenSent,
        NotifyInvoiceHasBeenRejected,
        NotifyInvoiceHasBeenReturned,
        NotifyInvoiceHasBeenFinished,
        NotifyInvoiceOpenPOHasBeenReceived,
        NotifyInvoicePOReviseHasBeenReceived,
        NotifyInvoiceNoReferenceHasBeenReceived

    }
}
