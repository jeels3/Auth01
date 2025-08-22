using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradesCompany_AW.Domain.Entities
{
    public class Address
    {
        public int id {  get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PinCode { get; set; }
        public string Street { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}
