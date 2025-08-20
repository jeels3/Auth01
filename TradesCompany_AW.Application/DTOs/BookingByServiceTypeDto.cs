using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradesCompany_AW.Domain.Entities;

namespace TradesCompany_AW.Application.DTOs
{
    public class BookingByServiceTypeDto
    {
        public int Id { get; set; }
        public int ServiceTypeId { get; set; }
        public string UserId { get; set; }
        public string customerName { get; set; }
        public string WorkDetails { get; set; } = null!;
        public decimal Price { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? CompletedAt { get; set; }
        public string Status { get; set; } = "Pending";
    }
}
