using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradesCompany_AW.Application.DTOs
{
    public class ProfileDto
    {
        public string Id {  get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public int? AddressId { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? PinCode { get; set; }
        public string? Street { get; set; }
    }
}
