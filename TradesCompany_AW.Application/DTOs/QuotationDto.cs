using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradesCompany_AW.Domain.Entities;

namespace TradesCompany_AW.Application.DTOs
{
    public class QuotationDto
    {
        public int? Id { get; set; }
        public int BookingId { get; set; }
        //public int ServiceManId { get; set; }
        public string ServiceDetails { get; set; }
        public decimal Price { get; set; }
    }
}
