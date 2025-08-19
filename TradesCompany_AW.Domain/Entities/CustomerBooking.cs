using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradesCompany_AW.Domain.Entities
{
    public class CustomerBooking
    {
        public int id { get; set; }
        public int ServiceTypeId { get; set; }
        public ServiceType ServiceType { get; set; } = null!;
        public string UserId { get; set; }
        public ApplicationUser User { get; set; } = null!;
        public string WorkDetails { get; set; } = null!;
        public decimal Price { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? CompletedAt { get; set; }
        public string Status { get; set; } = "Pending";
    }
}
