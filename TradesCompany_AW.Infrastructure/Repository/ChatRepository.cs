using Microsoft.AspNetCore.Mvc;
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
    public class ChatRepository : IChatRepository
    {
        public readonly ApplicationDbContext _context;

        public ChatRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<UserListForChatDto>> UserListingForChat(string currentUserId)
        {
            var result = await _context.Users
                .Select(user => new
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,

                    // Count unread messages sent by this user TO current user
                    UnreadCount = _context.channelMessages
                        .Where(msg => msg.SenderId == user.Id) // messages sent by this user
                        .Count(msg => !_context.isSeens
                            .Any(seen =>
                                seen.ChannelMessageId == msg.Id &&
                                seen.ReceiverId == currentUserId &&
                                seen.Seen))
                })
                .ToListAsync();
            return new List<UserListForChatDto>();
        }
    }
}
