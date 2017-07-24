using PayU.Core;
using PayU.LibraryModel;
using System;

namespace PayU.PaymentModel
{
    public class PayRequest
    {
        //Constructor'da SaleModel'i zorunlu kıldım. SaleModel verilmeden PayRequest türetilmesin.
        public PayRequest(SaleModel saleModel)
        {
            SaleModel = saleModel;
            CreditCardModel = new CreditCard();
        }
        public SaleModel SaleModel { get; set; }
        public CreditCard CreditCardModel { get; set; }
    }
    public class RefundRequest
    {
        //Refund iade alırken SaleModel'den bağımsız yaptım. SaleModel üzerinden kullanacağımız çok fazla değer yok.
        //Refund işlemi sırasında PayU DLL'lerini kullanmıyoruz. Web Client ile request atıp cevap alacağız.
        public RefundRequest()
        {
            Merchant = "";
            Hash = "";
            Currency = "TRY";
            IRNDate = DateTime.Now;
            OrderId = 0;
            OrderAmount = 0;

        }
        [Parameter(Name = "MERCHANT")]
        public string Merchant { get; set; }
        [Parameter(Name = "ORDER_REF")]
        public int OrderId { get; set; }
        /// <summary>
        /// Siparişin Toplam Tutarı
        /// </summary>
        [Parameter(Name = "ORDER_AMOUNT")]
        public decimal OrderAmount { get; set; }
        /// <summary>
        /// Örn: TRY, USD
        /// </summary>
        [Parameter(Name = "ORDER_CURRENCY")]
        public string Currency { get; set; }
        /// <summary>
        /// İade yapılmak istenilen tutar
        /// </summary>
        [Parameter(Name = "AMOUNT")]
        public decimal Amount { get; set; }
        /// <summary>
        /// İade Tarihi
        /// </summary>
        [Parameter(Name = "IRN_DATE")]
        public DateTime IRNDate { get; set; }
        [Parameter(Name = "ORDER_HASH")]
        public string Hash { get; set; }
    }
}