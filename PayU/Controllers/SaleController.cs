using PayU.BaseController;
using PayU.LibraryModel;
using PayU.Models;
using System;
using System.Linq;
using System.Web.Mvc;

namespace PayU.Controllers
{
    public class SaleController : Controller
    {
        // GET: Sale
        public ActionResult Index()
        {
            var saleModel = BaseController.SaleController.GetSale();
            return View(saleModel);
        }
        public ActionResult Basket()
        {
            var saleModel = BaseController.SaleController.GetSale();
            if (!saleModel.Products.Any())
                return RedirectToAction("Index");
            return View(saleModel);
        }
        public ActionResult Address()
        {
            var saleModel = BaseController.SaleController.GetSale();

            if (!saleModel.Products.Any())
                return RedirectToAction("Index");
            var addressModel = new AddressPageModel();
            addressModel.Sale = saleModel;
            return View(addressModel);
        }
        [HttpPost]
        public ActionResult Address(AddressPageModel model)
        {
            var saleModel = BaseController.SaleController.GetSale();
            var deliveryModel = new AddressModel
            {
                Address = model.Delivery.Address,
                City = model.Delivery.City,
                Country = model.Delivery.Country,
                ContactName = model.Delivery.ContactName,
                Name = model.Delivery.Name,
                Phone = model.Delivery.Phone,
                PostCode = model.Delivery.PostCode,
                Town = model.Delivery.Town,
                Type = Enum.AddressType.Delivery
            };
            saleModel.Address.Add(deliveryModel);

            if (model.IsDelivery)
            {
                //Fatura ve Teslimat adresi aynı olucaksa aynı adresin tipini dğeiştirip ekle.
                var invoiceModel = new AddressModel
                {
                    Address = model.Delivery.Address,
                    City = model.Delivery.City,
                    Country = model.Delivery.Country,
                    ContactName = model.Delivery.ContactName,
                    Name = model.Delivery.Name,
                    Phone = model.Delivery.Phone,
                    PostCode = model.Delivery.PostCode,
                    Town = model.Delivery.Town,
                    Type = Enum.AddressType.Invoice
                };
                saleModel.Address.Add(invoiceModel);
            }
            else
            {
                var invoiceModel = new AddressModel
                {
                    Address = model.Invoice.Address,
                    City = model.Invoice.City,
                    Country = model.Invoice.Country,
                    ContactName = model.Invoice.ContactName,
                    Name = model.Invoice.Name,
                    Phone = model.Invoice.Phone,
                    PostCode = model.Invoice.PostCode,
                    Town = model.Invoice.Town,
                    Type = Enum.AddressType.Invoice
                };
                saleModel.Address.Add(invoiceModel);
            }
            return RedirectToAction("Payment");
        }
        public ActionResult Payment()
        {
            var saleModel = BaseController.SaleController.GetSale();
            if (!saleModel.Products.Any())
                return RedirectToAction("Index");
            return View(saleModel);
        }
        [HttpPost]
        public ActionResult PaymentOk()
        {
            var saleModel = BaseController.SaleController.GetSale();

            Repository.Repository rep = new Repository.Repository();

            var result = rep.Pay(new PaymentModel.PayRequest(saleModel)
            {
                CreditCardModel = new PaymentModel.CreditCard
                {
                    CreditCardName = "Samet ÇINAR",
                    CardNumber = "5571135571135575",
                    ExpiryMonth = "12",
                    ExpiryYear = "2018",
                    CVV = "000"
                }
            });

            if (result.IsPay)
            {
                //Ödeme başarılı ise..
                var paymentModel = new LibraryModel.PaymentModel
                {
                    Name = "PayU Ödeme",
                    Price = saleModel.RemainingPrice
                };
                saleModel.AddPayment(paymentModel);
                saleModel.SaveChanges();
            }
            return RedirectToAction("Result");
        }
        public ActionResult Result()
        {
            //Burda kendimiz Session'a bakalım yeni nesne oluşmasın..
            if (Session["SaleModel"] != null)
            {
                var saleModel = (SaleModel)Session["SaleModel"];
                if (saleModel.RemainingPrice == 0)
                {
                    Session.Remove("SaleModel");
                    return View(saleModel);
                }
            }
            return RedirectToAction("Index");
        }
        public ActionResult AddProduct(string productName, string productBarcode, decimal productPrice, int productQuantity)
        {
            var saleModel = BaseController.SaleController.GetSale();
            for (int i = 0; i < productQuantity; i++)
            {
                saleModel.AddProduct(new LibraryModel.ProductModel
                {
                    Barcode = productBarcode,
                    Name = productName,
                    Quantity = 1,
                    Price = productPrice,
                    UniqId = Guid.NewGuid().ToString()
                });
            }

            saleModel.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult DeleteProduct(string uniqId)
        {
            var saleModel = BaseController.SaleController.GetSale();
            var productModel = saleModel.Products.FirstOrDefault(s => s.UniqId == uniqId);
            saleModel.DeleteProduct(productModel);
            saleModel.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult AddDiscount(string discountName, string discountCouponCode, decimal discountPrice)
        {
            var saleModel = BaseController.SaleController.GetSale();
            saleModel.AddDiscount(new LibraryModel.DiscountModel
            {
                Price = discountPrice,
                Name = discountName,
                CouponCode = discountCouponCode,
                UniqId = Guid.NewGuid().ToString()
            });
            saleModel.SaveChanges();
            return RedirectToAction("Basket");
        }
        public ActionResult DeleteDiscount(string uniqId)
        {
            var saleModel = BaseController.SaleController.GetSale();
            var discountModel = saleModel.Discounts.FirstOrDefault(s => s.UniqId == uniqId);
            saleModel.DeleteDiscount(discountModel);
            saleModel.SaveChanges();
            return RedirectToAction("Basket");
        }
        public ActionResult AddPayment(string paymentName, decimal paymentPrice)
        {
            var saleModel = BaseController.SaleController.GetSale();
            saleModel.AddPayment(new PayU.LibraryModel.PaymentModel
            {
                Price = paymentPrice,
                Name = paymentName
            });
            saleModel.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}