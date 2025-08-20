using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradesCompany_AW.Domain.Entities;

namespace TradesCompany_AW.Application.Repository
{
    public interface INotificationRepository
    {
        public Task<int> GetCountByUserId(string userId);

        public Task<List<Notification>> GetNotificationByUserId(string userId);
        public Task<bool> NotificatinSeen(string userId , int notificationId);
    }
}
