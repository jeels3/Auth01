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
    public class ProfileRepository : IProfileRepository
    {
        private readonly ApplicationDbContext _context;

        public ProfileRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ProfileDto> GetProfileDetails(string userId)
        {
            return await _context.Users.Where(u => u.Id == userId).
                Include(u => u.Address).
                Select(u => new ProfileDto
                {
                    UserName = u.UserName,
                    Id = u.Id,
                    Email = u.Email,
                    State = u.Address.State,
                    City = u.Address.City,
                    PinCode = u.Address.PinCode,
                    Street = u.Address.PinCode,
                    AddressId = u.Address.id
                }).FirstOrDefaultAsync();
        }
    }
}
