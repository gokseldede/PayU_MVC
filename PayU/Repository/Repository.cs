using PayU.Core;
using PayU.LibraryModel;
using PayU.PaymentHelper;
using PayU.PaymentModel;
using System;
using System.Linq;
using System.Text;
using System.Web;

namespace PayU.Repository
{
    //Örnek kart bilgileri..
    //MERCHANT=OPU_TEST
    //SECRET_KEY=SECRET_KEY 
    //Test Kartları: 
    //Akbank:
    //Kart Numarası (Visa) : 4355084355084358 
    //Kart Numarası (Master Card) : 5571135571135575
    //Son Kullanma Tarihi :12/18
    //Güvenlik Numarası : 000 
    //--
    //Tarih formatının (UTC/GMT) (Türkiye saatine göre 3 saat geri) olacak şekilde gönderilmesini sağlayınız. 
    public class Repository
    {
        public string signatureKey = "SECRET_KEY";
        public string merchant = "OPU_TEST";
        public string returnUrl = "http://localhost:55493/Sale/Result";
        public string refundEndPoint = "https://secure.payu.com.tr/order/lu.php";
        /// <param name="goPayUPage">Bu alan true olursa eğer PayU sayfasına gider..</param>
        public PayResult Pay(PayRequest request, bool? goPayUPage = false)
        {
            if (goPayUPage == false)
                return PayPayU(request);
            else
                GoPayUPage(request.SaleModel);

            return new PayResult();
        }
        /// <summary>
        /// PayU 3D yönlendirme isteği ile siparişin tamamlanmasını isteyebilir. Bankadan gelen HttpRequest'i verip Result'ı al..
        /// </summary>
        /// <param name="request">3D bankadan gelen request</param>
        public PayResult Pay3DResult(HttpRequest request)
        {
            try
            {
                var response = PayU.AutomaticLiveUpdate.ALUResponse.FromHttpRequest(request);
                return new PayResult
                {
                    BankOrderId = response.BankClientId,
                    Amount = response.Amount ?? 0,
                    ErrorMessage = response.ReturnMessage,
                    AuthCode = response.AuthCode,
                    IsPay = response.IsSuccess,
                    CreditCardNumber = response.PAN,
                    Order = response.BankOrderId,
                    TransactionId = response.TransactionId,
                    InstallmentCount = Convert.ToByte(response.InstallmentNumber),
                    CreditCardName = response.CardProgramName
                };
            }
            catch (PayuException ex)
            {
                //Exception loglanabilir..
                return new PayResult();
            }
        }
        /// <summary>
        /// Çekim için PayU tarafına gönderir..
        /// </summary>
        private void GoPayUPage(SaleModel saleModel)
        {
            try
            {
                var service = new LiveUpdate.LiveUpdateService(signatureKey);
                var order = new LiveUpdate.OrderDetails();
                order.Merchant = merchant;
                order.OrderRef = saleModel.Code.ToString();
                order.OrderDate = DateTime.Now;
                order.ReturnUrl = returnUrl;
                //Ürünleri dön..
                foreach (var orderDetail in saleModel.Products)
                {
                    var product = new LiveUpdate.ProductDetails
                    {
                        Code = orderDetail.Barcode,
                        Name = orderDetail.Name,
                        Quantity = 1,
                        VAT = 8,
                        UnitPrice = orderDetail.Price,
                        Information = string.Empty,
                        PriceType = LiveUpdate.PriceType.GROSS,
                    };
                    order.ProductDetails.Add(product);
                }

                order.ShippingCosts = saleModel.TotalCargo;
                var saleInvoiceAddress = saleModel.Address.FirstOrDefault(s => s.Type == Enum.AddressType.Invoice);
                var saleDeliveryAddress = saleModel.Address.FirstOrDefault(s => s.Type == Enum.AddressType.Delivery);
                order.DestinationCity = saleInvoiceAddress.Town;
                order.DestinationState = saleDeliveryAddress.City;
                order.DestinationCountry = "TR";

                order.BillingDetails = new PayU.LiveUpdate.BillingDetails
                {
                    FirstName = "Göksel",
                    LastName = "DEDE",
                    Email = "g.dede@hiperaktif.com",
                    City = saleInvoiceAddress.Town, // Ilce/Semt
                    State = saleInvoiceAddress.City, // Sehir
                    CountryCode = "TR"
                };
                var sb = new StringBuilder();
                //PayU tarafına gönderim işlemleri..
                sb.Append("<html xmlns=\"http://www.w3.org/1999/xhtml\">");
                sb.Append("<head runat=\"server\"></head>");
                sb.Append(string.Format("<body onload=\"document.{0}.submit()\">", "payForm"));
                string htmlForm = service.RenderPaymentForm(order, "PayU ile Ödeme Yap");
                sb.Append(htmlForm);
                sb.Append("</body>");
                sb.Append("</html>");
                HttpContext.Current.Response.Clear();
                HttpContext.Current.Response.ContentType = "text/HTML";
                HttpContext.Current.Response.Write(sb.ToString());
                HttpContext.Current.Response.End();
            }
            catch (PayuException ex)
            {

                //Exception loglanabilir..
            }

        }
        /// <summary>
        /// PayU ekranı açmadan direkt çekim yapmak için..
        /// </summary>
        private PayResult PayPayU(PayRequest requestModel)
        {
            try
            {
                var saleModel = requestModel.SaleModel;
                var service = new PayU.AutomaticLiveUpdate.ALUService(signatureKey);
                var order = new PayU.AutomaticLiveUpdate.OrderDetails();
                order.Merchant = merchant;
                order.OrderRef = saleModel.Code.ToString();
                order.OrderDate = DateTime.Now.AddMinutes(10); //DateTime.Now; Sunucular arasında dakika farkı olduğu için 10 dakika ileri tarih gönderiyorum localde problem çıkmasın.
                order.ReturnUrl = returnUrl;// string.Empty;
                order.PricesCurrency = "TRY";
                order.ClientIpAddress = "85.103.39.68"; //Müşteriden gelen ip adresi

                //Ürünleri dön..
                foreach (var orderDetail in saleModel.Products)
                {
                    var product = new PayU.AutomaticLiveUpdate.ProductDetails
                    {
                        Code = orderDetail.Barcode,
                        Name = orderDetail.Name,
                        Quantity = 1,
                        UnitPrice = orderDetail.Price,
                        Information = string.Empty,
                    };
                    order.ProductDetails.Add(product);
                }
                var creditCardModel = requestModel.CreditCardModel;
                order.CardDetails = new PayU.AutomaticLiveUpdate.CardDetails
                {
                    CardNumber = creditCardModel.CardNumber,
                    ExpiryMonth = creditCardModel.ExpiryMonth,
                    ExpiryYear = creditCardModel.ExpiryYear,
                    CVV = creditCardModel.CVV,
                    CardOwnerName = creditCardModel.CreditCardName
                };
                order.SelectedInstallmentNumber = creditCardModel.Installment;
                var saleInvoiceAddress = saleModel.Address.FirstOrDefault(s => s.Type == PayU.Enum.AddressType.Invoice);
                var saleDeliveryAddress = saleModel.Address.FirstOrDefault(s => s.Type == PayU.Enum.AddressType.Delivery);
                order.BillingDetails = new PayU.AutomaticLiveUpdate.BillingDetails
                {
                    FirstName = "Göksel",
                    LastName = "DEDE",
                    Email = "g.dede@hiperaktif.com",
                    City = saleInvoiceAddress.Town, // Ilce/Semt
                    State = saleInvoiceAddress.City, // Sehir
                    CountryCode = "TR",
                    ZipCode = saleInvoiceAddress.PostCode,
                    Address = saleInvoiceAddress.Address,
                    PhoneNumber = saleInvoiceAddress.Phone
                };

                order.DeliveryDetails = new PayU.AutomaticLiveUpdate.DeliveryDetails
                {
                    FirstName = "Göksel",
                    LastName = "DEDE",
                    Email = "g.dede@hiperaktif.com",
                    City = saleDeliveryAddress.Town, // Ilce/Semt
                    State = saleDeliveryAddress.City, // Sehir
                    CountryCode = "TR",
                    ZipCode = saleDeliveryAddress.PostCode,
                    Address = saleDeliveryAddress.Address,
                    PhoneNumber = saleDeliveryAddress.Phone
                };

                var response = service.ProcessPayment(order);
                //PayU işlemin 3D devam etmesini isteyebilir..
                if (response.Is3DSResponse)
                {
                    HttpContext.Current.Response.Redirect(response.Url3DS);
                    HttpContext.Current.Response.End();
                }

                if (response.Status == PayU.AutomaticLiveUpdate.Status.Success)
                {

                    return new PayResult
                    {
                        BankOrderId = response.BankClientId,
                        Amount = response.Amount ?? 0,
                        ErrorMessage = response.ReturnMessage,
                        AuthCode = response.AuthCode,
                        IsPay = true,
                        CreditCardNumber = response.PAN,
                        Order = response.BankOrderId,
                        TransactionId = response.TransactionId,
                        InstallmentCount = Convert.ToByte(response.InstallmentNumber),
                        CreditCardName = response.CardProgramName
                    };
                    //İşlem başarılı..

                }
                return new PayResult
                {
                    ErrorMessage = response.ReturnMessage,
                    IsPay = false
                };

            }
            catch (PayuException ex)
            {
                //Exception loglanabilir..
            }
            return new PayResult();

        }
        public RefundResult PayCancel(RefundRequest refund)
        {
            try
            {
                refund.Merchant = merchant;
                var parameterHandler = new ParameterHandler(refund);
                var hashData = parameterHandler.CreateOrderRequestHash(signatureKey);
                var requestData = parameterHandler.GetRequestData();
                var requestHelper = new RequestHelper<RefundResult>();
                var result = requestHelper.SendRequest(refundEndPoint, requestData);
                return result;
            }
            catch (PayuException ex)
            {

                //Exception loglanabilir..
            }
            return new RefundResult();
        }

    }
}