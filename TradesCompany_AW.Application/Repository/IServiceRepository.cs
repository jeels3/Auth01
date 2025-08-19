using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradesCompany_AW.Domain.Entities;

namespace TradesCompany_AW.Application.Repository
{
    public interface IServiceRepository
    {
        Task<ServiceType> GetServiceByServiceName(string serviceName);
    }
}
