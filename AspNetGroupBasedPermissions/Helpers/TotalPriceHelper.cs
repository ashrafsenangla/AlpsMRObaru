using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AspNetGroupBasedPermissions.Helpers
{
    public static class TotalPriceHelper
    {
        public static decimal Count(string quantityString, string unitPriceString, string discountString, string taxString,
            string otherChargeString, string discountPercentageString, string taxPercentageString)
        {
            var quantity = Decimal.Parse(quantityString);
            var unitPrice = Decimal.Parse(unitPriceString);
            var discount = Decimal.Parse(discountString);
            var tax = Decimal.Parse(taxString);
            var otherCharge = Decimal.Parse(otherChargeString);
            var discountPercentage = Decimal.Parse(discountPercentageString);
            var taxPercentage = Decimal.Parse(taxPercentageString);
            return Count(quantity, unitPrice, discount, tax, otherCharge,
            discountPercentage, taxPercentage);
        }
        public static decimal Count(decimal quantity, decimal unitPrice, decimal discount, decimal tax, decimal otherCharge,
            decimal discountPercentage, decimal taxPercentage)
        {
            var totalRawPrice = quantity * unitPrice;
            var totalPrice = totalRawPrice - discount + tax + otherCharge;
            totalPrice = totalPrice - (totalRawPrice * discountPercentage / 100);
            totalPrice = totalPrice + +(totalRawPrice * taxPercentage / 100);
            return totalPrice;
        }
    }
}