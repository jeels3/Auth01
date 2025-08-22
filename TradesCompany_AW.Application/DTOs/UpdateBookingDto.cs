using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradesCompany_AW.Application.DTOs
{
    public class UpdateBookingDto
    {
        public int id { get; set; }
        public decimal price { get; set; }
        public string workDetails { get; set; }
    }
}
