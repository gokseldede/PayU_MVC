using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PayU.LibraryModel
{
    public class DiscountModel
    {
        public string UniqId { get; set; }
        public string Name { get; set; }
        public string CouponCode { get; set; }
        public decimal Price { get; set; }
    }
}