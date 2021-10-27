using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace AspNetGroupBasedPermissions.Helpers.Enums
{
    public enum EPRStatusesEnum
    {
        [Description("Draft")]
        Draft = 1,
        [Description("Submit MSR")]
        SubmitMSR = 2,
        [Description("Pending MSR Confirmation")]
        PendingRFQ = 3,
        [Description("Pending PC")]
        PendingPC = 4,
        [Description("Pending PR")]
        PendingPR = 5,
        DraftPR = 6,
        SubmitPR = 7,
        RecommendPR = 8,
        PendingApproval = 9,
        ApprovedPR = 10,
    }
}