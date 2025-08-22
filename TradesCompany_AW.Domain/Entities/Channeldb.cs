using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradesCompany_AW.Domain.Entities
{
    public class Channeldb
    {
        public int Id { get; set; }
        public string ChannelName { get; set; }
        public string CreatorId { get; set; }
        public ApplicationUser User { get; set; }
        public List<ChannelUser> ChannelUsers { get; set; } = new List<ChannelUser>();
        //public string manil { get; set; } // false
        //public string? tushar { get; set; } // true
        //public string Milan { get; set; } = null!; // false
    }
}
