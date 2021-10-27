using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Framework.Const
{
    public static class InventoryTransStatuses
    {
        public const string PURCHASE_ORDER = "PO";
        public const string STOCK_ISSUANCE = "Issuance";
        public const string STOCK_RETURN = "Return";
        public const string STOCK_ADJUSTMENT = "Adjustment";
        public const string TRANSFER_ORDER_SEND = "TO Send";
        public const string TRANSFER_ORDER_RECEIVE = "TO Receive";
        public const string TRANSFER_ORDER_REJECT = "TO Reject";
        public const string BACK_LOAD_SEND = "BL Send";
        public const string BACK_LOAD_RECEIVE = "BL Receive";
        public const string BACK_LOAD_REJECT = "BL Reject";
        public const string MANIFEST_SEND = "MTF Send";
        public const string MANIFEST_RECEIVE = "MTF Receive";
        public const string MANIFEST_REJECT = "MTF Reject";
    }
}
