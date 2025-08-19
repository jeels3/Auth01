using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradesCompany_AW.Domain.Entities
{
    public class ServiceMan
    {
        public int id {  get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public int ServiceTypeId { get; set; }
        public ServiceType ServiceTypes { get; set; }
    }
}
