using TradesCompany_AW.Domain.Entities;

namespace TradesCompany_AW.Web.ViewModel
{
    public class CustomerBookingViewModel
    {
        public int? id { get; set; }
        public int ServiceTypeId { get; set; }
        public string? UserId { get; set; }
        public string WorkDetails { get; set; } = null!;
        public decimal Price { get; set; }
        public DateTime? CreatedAt { get; set; } = DateTime.Now;
        public DateTime? CompletedAt { get; set; }
        public string Status { get; set; } = "Pending";
    }
}
