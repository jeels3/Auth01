using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradesCompany_AW.Shared.Hubs
{
    public class NotificationHub : Hub
    {
        /*public async Task SendNotification(string employeeId, string message)
        {
            await Clients.User(employeeId).SendAsync("ReceiveNotification", message);
        }*/

        public Task Hello()
        {
            Console.WriteLine("Message Come");
            return Task.CompletedTask;
        }
        public override async Task OnConnectedAsync()
        {
            Console.WriteLine($"Connected UserIdentifier: {Context.UserIdentifier}");
            await base.OnConnectedAsync();
        }
    }
}
