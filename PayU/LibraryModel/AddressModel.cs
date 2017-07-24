using PayU.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PayU.LibraryModel
{
    public class AddressModel
    {
        public AddressType Type { get; set; }
        /// <summary>
        /// Adres Adı
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Fatura, Telsimat Adresinde yazılacak isim
        /// </summary>
        public string ContactName { get; set; }
        public string PostCode { get; set; }
        public string Phone { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Town { get; set; }
        public string Address { get; set; }
    }
}