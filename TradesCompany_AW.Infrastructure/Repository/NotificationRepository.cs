using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradesCompany_AW.Application.Repository;
using TradesCompany_AW.Domain.Entities;
using TradesCompany_AW.Infrastructure.Data;

namespace TradesCompany_AW.Infrastructure.Repository
{
    public  class NotificationRepository  : INotificationRepository
    {
        private readonly ApplicationDbContext _context;

        public NotificationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> GetCountByUserId(string userId)
        {
            return await _context.notifications.Where(n => n.userId == userId && n.IsRead == false).CountAsync();
        }

        public async Task<List<Notification>> GetNotificationByUserId(string userId)
        {
            return await _context.notifications.Where(n => n.userId == userId && n.IsRead == false).OrderByDescending(n => n.CreatedAt).ToListAsync();
        }

        public async Task<bool> NotificatinSeen(string userId, int notificationId)
        {
            var notification = await _context.notifications
                .FirstOrDefaultAsync(n => n.userId == userId && n.Id == notificationId);

            if (notification == null)
            {
                return false;
            }

            notification.IsRead = true;
            _context.notifications.Update(notification);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
