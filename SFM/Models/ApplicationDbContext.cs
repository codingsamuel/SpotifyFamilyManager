using Microsoft.EntityFrameworkCore;

namespace SFM.Models
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<SpotifyUser> SpotifyUsers { get; set; }

        public DbSet<Subscription> Subscriptions { get; set; }

        public DbSet<Config> Configs { get; set; }

        public DbSet<UserAddress> UserAddresses { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseNpgsql("Host=localhost;Database=sfm;Username=postgres;Password=abc123");
        }
    }
}