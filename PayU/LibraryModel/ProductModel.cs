using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PayU.LibraryModel
{
    public class ProductModel
    {
        public string UniqId { get; set; }
        public string Name { get; set; }
        public string Barcode { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}