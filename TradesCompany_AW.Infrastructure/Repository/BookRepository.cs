using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradesCompany_AW.Application.DTOs;
using TradesCompany_AW.Application.Repository;
using TradesCompany_AW.Infrastructure.Data;

namespace TradesCompany_AW.Infrastructure.Repository
{
    public class BookRepository : IBookingRepository
    {
        private readonly ApplicationDbContext _context;

        public BookRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<BookingByServiceTypeDto>> GetAllBookingByServiceType (int serviceTypeId)
        {
            return await _context.customerBookings.Where(cb => cb.ServiceTypeId == serviceTypeId)
                .Include(cb => cb.User)
                .Select(cb => new BookingByServiceTypeDto
                {
                    Id = cb.id,
                    ServiceTypeId = cb.ServiceTypeId,
                    UserId = cb.UserId,
                    customerName = cb.User.UserName,
                    WorkDetails = cb.WorkDetails,
                    Price = cb.Price,
                    CreatedAt = cb.CreatedAt,
                    CompletedAt = cb.CompletedAt,
                    Status = cb.Status,

                })
                .ToListAsync();
        }
    }
}
