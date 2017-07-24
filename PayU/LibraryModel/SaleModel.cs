using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PayU.LibraryModel
{
    public class SaleModel
    {
        public SaleModel()
        {
            Products = new List<ProductModel>();
            Payments = new List<PaymentModel>();
            Discounts = new List<DiscountModel>();
            Address = new List<AddressModel>();
        }

        #region Properties
        public string Code { get; set; }
        public List<ProductModel> Products { get; set; }
        public decimal GrandTotal
        {
            get
            {
                return Products.Sum(s => s.Price);
            }
        }
        public decimal TotalQuantity
        {
            get
            {
                if (Products.Any())
                    return Products.Sum(s => s.Quantity);

                return decimal.Zero;
            }
        }
        public List<PaymentModel> Payments { get; set; }
        /// <summary>
        /// Yapılan ödemelerin toplamı
        /// </summary>
        public decimal TotalPayment
        {
            get
            {
                if (Payments.Any())
                    return Payments.Sum(s => s.Price);
                return decimal.Zero;
            }
        }
        public List<DiscountModel> Discounts { get; set; }
        /// <summary>
        /// Yapılan indirimlerin toplamı
        /// </summary>
        public decimal TotalDiscount
        {
            get
            {
                if (Discounts.Any())
                    return Discounts.Sum(s => s.Price);
                return decimal.Zero;
            }
        }
        /// <summary>
        /// Yapılan ödemeler ile toplam tutarın farkı.
        /// Kalan tutar
        /// </summary>
        public decimal RemainingPrice
        {
            get
            {
                return GrandTotal - (TotalPayment + TotalDiscount);
            }
        }
        public decimal TotalCargo { get; set; }
        public List<AddressModel> Address { get; set; }
        #endregion
        #region Methods
        /// <summary>
        /// İlgili SaleModel'e bir satır ürün ekler..
        /// </summary>
        public void AddProduct(ProductModel product)
        {
            this.Products.Add(product);
        }
        /// <summary>
        /// İlgili SaleModel üzerinden bir satır ürün siler..
        /// </summary>
        /// <param name="uniqId"></param>
        public void DeleteProduct(ProductModel product)
        {
            this.Products.Remove(product);
        }
        /// <summary>
        /// İlgili SaleModel'e bir satır ödeme ekler..
        /// </summary>
        public void AddPayment(PaymentModel payment)
        {
            this.Payments.Add(payment);
        }
        /// <summary>
        /// İlgili SaleModel'e bir satır indirim ekler..
        /// </summary>
        public void AddDiscount(DiscountModel discount)
        {
            this.Discounts.Add(discount);
        }
        /// <summary>
        /// İlgili SaleModel'den bir satır indirim siler
        /// </summary>
        /// <param name="discount"></param>
        public void DeleteDiscount(DiscountModel discount)
        {
            this.Discounts.Remove(discount);
        }
        /// <summary>
        /// İlgili SaleModel'e bir satır adress ekler
        /// </summary>
        /// <param name="address"></param>
        public void AddAddress(AddressModel address)
        {
            this.Address.Add(address);
        }
        #endregion
    }
}