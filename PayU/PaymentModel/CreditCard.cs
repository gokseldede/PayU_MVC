using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PayU.PaymentModel
{
    public class CreditCard
    {
        public string CardNumber { get; set; }
        public string ExpiryMonth { get; set; }
        public string ExpiryYear { get; set; }
        public string CVV { get; set; }
        public string CreditCardName { get; set; }
        public int Installment { get; set; }
    }
}