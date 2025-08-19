using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using System.Threading.Channels;
using TradesCompany_AW.Domain.Entities;

namespace TradesCompany_AW.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser , IdentityRole , string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Channeldb>().HasIndex(c => c.ChannelName).IsUnique();

            builder.Entity<RefreshToken>()
                .HasOne(rt => rt.User)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(rt => rt.UserId);

            builder.Entity<IsSeen>()
                .HasOne(i => i.ChannelMessage)
                .WithMany(c => c.IsSeen)
                .HasForeignKey(i => i.ChannelMessageId)
                .OnDelete(DeleteBehavior.Restrict);

        }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<ServiceType> serviceTypes { get; set; }
        public DbSet<CustomerBooking> customerBookings { get; set; }
        public DbSet<ServiceMan> serviceMan { get; set; }
        public DbSet<Channeldb> channeldb { get; set; }
        public DbSet<ChannelUser> channelUsers { get; set; }
        public DbSet<ChannelMessage> channelMessages { get; set; }
        public DbSet<IsSeen> isSeens { get; set; }
        public DbSet<Notification> notifications { get; set; }

    }
}
