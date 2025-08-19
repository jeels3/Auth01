using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace TradesCompany_AW.Domain.Entities
{
    public class ChannelUser
    {
        public int Id { get; set; }
        public int ChannelId { get; set; }
        public Channeldb Channeldb { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}
