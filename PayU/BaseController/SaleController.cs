using PayU.LibraryModel;
using System;
using System.Web;

namespace PayU.BaseController
{
    public static class SaleController
    {
        public static SaleModel GetSale()
        {
            if (HttpContext.Current.Session["SaleModel"] != null)
                return (SaleModel)HttpContext.Current.Session["SaleModel"];
            Random rnd = new Random();
            return new SaleModel
            {
                Code = rnd.Next(2146, 99999999).ToString()
            };
        }
        public static void SaveChanges(this SaleModel saleModel)
        {
            HttpContext.Current.Session["SaleModel"] = saleModel;
        }
    }
}