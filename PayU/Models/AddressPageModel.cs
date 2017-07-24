namespace PayU.Models
{
    public class AddressPageModel
    {
        public AddressPageModel()
        {
            Invoice = new AddressItem();
            Delivery = new AddressItem();
            IsDelivery = true;
        }
        public LibraryModel.SaleModel Sale { get; set; }
        public AddressItem Invoice { get; set; }
        public AddressItem Delivery { get; set; }
        public bool IsDelivery { get; set; }
    }
    public class AddressItem
    {
        public string Name { get; set; }
        public string ContactName { get; set; }
        public string PostCode { get; set; }
        public string Phone { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Town { get; set; }
        public string Address { get; set; }
    }
}