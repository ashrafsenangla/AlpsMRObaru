using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AspNetGroupBasedPermissions.Helpers
{
    public static class AXWarehouse
    {
        public static string GetWarehouseFromSite(this String value)
        {
            var Warehouse = "";
            switch (value)
            {
                case "UOD-N02":
                    Warehouse = "UODN02-WH";
                    break;
                case "UOD-N03":
                    Warehouse = "UODN03-WH";
                    break;
                case "UOD-N04":
                    Warehouse = "UODN04-WH";
                    break;
                case "UOD-N05":
                    Warehouse = "UODN05-WH";
                    break;
                case "UOD-N06":
                    Warehouse = "UODN06-WH";
                    break;
                case "UOD-N07":
                    Warehouse = "UODN07-WH";
                    break;
                case "UOD-N08":
                    Warehouse = "UODN08-WH";
                    break;
                default: break;
            }
            return Warehouse;
        }

        public static string GetWarehouseClient(this String value)
        {
            var Warehouse = "";
            switch (value)
            {
                case "UOD-N02":
                    Warehouse = "UODN02-CL";
                    break;
                case "UOD-N03":
                    Warehouse = "UODN03-CL";
                    break;
                case "UOD-N04":
                    Warehouse = "UODN04-CL";
                    break;
                case "UOD-N05":
                    Warehouse = "UODN05-CL";
                    break;
                case "UOD-N06":
                    Warehouse = "UODN06-CL";
                    break;
                case "UOD-N07":
                    Warehouse = "UODN07-CL";
                    break;
                case "UOD-N08":
                    Warehouse = "UODN08-CL";
                    break;
                default: break;
            }
            return Warehouse;
        }
    }
}