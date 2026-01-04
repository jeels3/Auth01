using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using System.Threading.Channels;
using Auth01.Domain.Entities;

namespace Auth01.Infrastructure.Data
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

            builder.Entity<RefreshToken>()
                .HasOne(rt => rt.User)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(rt => rt.UserId);
        }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

    }
}
