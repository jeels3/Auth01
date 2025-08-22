using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradesCompany_AW.Domain.Entities
{
    public class Quotation
    {
        public int Id { get; set; }
        public int BookingId { get; set; }
        public CustomerBooking customerBooking { get; set; } = null!;
        public int ServiceManId { get; set; }
        public ServiceMan ServiceMan { get; set; } = null!;
        public string ServiceDetails { get; set; }
        public decimal Price { get; set; }
        public string Status { get; set; } = "Pending"; // ENUM: Pending, Accepted, Rejected
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
