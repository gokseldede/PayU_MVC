using PayU.Core;
using System;

namespace PayU.PaymentModel
{
    public class PayResult
    {
        public bool IsPay { get; set; }
        public string Order { get; set; }
        public decimal Amount { get; set; }
        public string AuthCode { get; set; }
        public string ErrorMessage { get; set; }
        public string CreditCardNumber { get; set; }
        public string CreditCardName { get; set; }
        public string TransactionId { get; set; }
        public string BankOrderId { get; set; }
        public byte InstallmentCount { get; set; }
    }
    public class RefundResult
    {

        [Parameter(Name = "ORDER_REF")]
        public int OrderId { get; set; }
        [Parameter(Name = "RESPONSE_CODE")]
        public string ResponseCode { get; set; }
        [Parameter(Name = "RESPONSE_MSG")]
        public string ResponseMsg { get; set; }
        [Parameter(Name = "IRN_DATE")]
        public DateTime IRNDate { get; set; }
        [Parameter(Name = "ORDER_HASH")]
        public string Hash { get; set; }
    }
}