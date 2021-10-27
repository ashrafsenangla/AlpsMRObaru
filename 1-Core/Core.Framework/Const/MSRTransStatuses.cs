using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Framework.Const
{
    public static class MSRTransStatuses
    {
        public const int DRAFT=1;
        public const int PENDING_MANAGER_APPROVAL=2;
        public const int MSR_RETURNED = 3;
        public const int MSR_REJECTED=4;
        public const int PENDING_CAPEX_APPROVAL=5;
        public const int CAPEX_RETURNED=6;
        public const int CAPEX_REJECTED=7;
        public const int PENDING_BUYER_ACCEPTENCE=8;
        public const int MSR_ACCEPTED=9;
        public const int RFQ_DRAFT = 10;
        public const int RFQ_ISSUED = 11;
        public const int PENDING_PRICE_COMPARISON=12;
        public const int PR_DRAFT=13;
        public const int PENDING_PO_APPROVAL=14;
        public const int PO_RETURNED=15;
        public const int PO_REJECTED=16;
        public const int PO_READY_TO_ISSUE=17;
        public const int PO_ISSUED=18;
        public const int PARTIAL_GRN=19;
        public const int FULL_GRN = 20;
        public const int PENDING_RECEIVE_ON_BOARD = 21;
        public const int PENDING_PR_SUBMISSION = 24;
    }
}
