using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradesCompany_AW.Application.Services
{
    public interface INotificationService
    {
        public Task SendBookingNotificationToEmployee(int servicetypeId , string message);
    }
}
