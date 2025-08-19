using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradesCompany_AW.Application.DTOs;

namespace TradesCompany_AW.Application.Repository
{
    public interface IChatRepository
    {
        public Task<List<UserListForChatDto>> UserListingForChat(string currentUserId);
    }
}
