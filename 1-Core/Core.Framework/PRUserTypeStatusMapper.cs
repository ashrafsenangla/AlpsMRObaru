using Core.Framework.Const;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Framework
{
    public static class PRUserTypeStatusMapper
    {
        public static string GetStatus(string userType)
        {
            switch (userType)
            {
                case PRUserType.REQUESTER: return "Pending Revision";
                case PRUserType.MANAGER: return "Pending Manager Approval";
                case PRUserType.BUYERADMIN: return "Pending Buyer Admin";
                case PRUserType.BUYER: return "Pending Buyer Acceptance";
                case PRUserType.APPROVER: return "Pending Approver";
                case PRUserType.CAPEXACCOUNTANT: return "Pending Accountant";
                case PRUserType.CAPEXAPRESIDENT: return "Pending President Approval";
                case PRUserType.ITVERIFIER: return "Pending IT Verifier";
                default: return "";
            }
        }
        public static string GetStatusPR(string userType)
        {
            switch (userType)
            {
                case PRUserType.REQUESTER: return "Pending Revision";
                case PRUserType.MANAGER: return "Pending Manager Approval";
                case PRUserType.BUYERADMIN: return "Pending Buyer Admin";
                case PRUserType.BUYER: return "Pending Revision";
                case PRUserType.APPROVER: return "Pending Approver";
                case PRUserType.CAPEXACCOUNTANT: return "Pending Accountant";
                case PRUserType.CAPEXAPRESIDENT: return "Pending President Approval";
                case PRUserType.ITVERIFIER: return "Pending IT Verifier";
                default: return "";
            }
        }
    }
}
