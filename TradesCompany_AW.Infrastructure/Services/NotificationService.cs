using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradesCompany_AW.Application.Services;
using TradesCompany_AW.Domain.Entities;
using TradesCompany_AW.Infrastructure.Data;
using TradesCompany_AW.Shared.Hubs;

namespace TradesCompany_AW.Infrastructure.Services
{
    public class NotificationService : INotificationService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<NotificationHub> _hubContext;
         

        public NotificationService(ApplicationDbContext context , IHubContext<NotificationHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        public async Task SendBookingNotificationToEmployee(int serviceTypeId, string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentException("Message cannot be null or empty.", nameof(message));

            // get all employees for this service type
            var users = await _context.serviceMan
                .Where(sm => sm.ServiceTypeId == serviceTypeId)
                .Include(sm => sm.User)
                .ToListAsync();

            if (users == null || !users.Any())
            {
                // No employees found for this service type
                return;
            }

            var notifications = new List<Notification>();

            foreach (var user in users)
            {
                if (string.IsNullOrEmpty(user.UserId))
                    continue; // skip invalid users

                var notification = new Notification
                {
                    userId = user.UserId,
                    NotificationType = "Service Booking",
                    Message = message,
                    CreatedAt = DateTime.UtcNow,
                    IsRead = false
                };

                notifications.Add(notification);
                try
                {
                    await _hubContext.Clients.User(user.UserId)
                        .SendAsync("ReceiveNotification", message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"SignalR error for user {user.UserId}: {ex.Message}");
                }
            }

            if (notifications.Any())
            {
                await _context.notifications.AddRangeAsync(notifications);

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    // log persistence issue
                    Console.WriteLine($"DB Save error: {ex.Message}");
                }
            }
        }

    }
}
