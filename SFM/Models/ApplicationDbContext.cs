using Microsoft.EntityFrameworkCore;

namespace SFM.Models
{
    public class ApplicationDbContext : DbContext
    {

        public DbSet<SpotifyUser> SpotifyUsers { get; set; }

        public DbSet<Subscription> Subscriptions { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseMySQL("server=localhost;database=sfm;user=root;");
        }
    }
}