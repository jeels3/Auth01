using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TradesCompany_AW.Application.Repository;
using TradesCompany_AW.Domain.Entities;
using TradesCompany_AW.Infrastructure.Data;

namespace TradesCompany_AW.Infrastructure.Repository
{
    public class ServiceRepository : IServiceRepository
    {
        private readonly ApplicationDbContext _context;

        public ServiceRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<ServiceType> GetServiceByServiceName(string serviceName)
        {
            return await _context.serviceTypes
                .FirstOrDefaultAsync(s => s.ServiceName == serviceName);
        }

        public async Task<int> GetServiceTypeByUserId(string userId)
        {
            return await _context.serviceMan.Where(sm => sm.UserId == userId).Select(sm => sm.ServiceTypeId).FirstOrDefaultAsync();
        }
    }

}
