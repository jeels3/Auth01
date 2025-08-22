using TradesCompany_AW.Domain.Entities;

namespace TradesCompany_AW.Web.ViewModel
{
    public class AddressViewModel
    {
        public int? AddressId { get; set;}
        public string City { get; set; }
        public string State { get; set; }
        public string PinCode { get; set; }
        public string Street { get; set; }
        public string? UserId { get; set; }
    }
}
