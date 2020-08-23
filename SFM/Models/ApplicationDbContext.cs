using System;
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

            var isDevelopment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";
            if (isDevelopment)
                optionsBuilder.UseMySQL("host=localhost;database=sfm;username=root;password=");
            else
                optionsBuilder.UseMySQL(
                    "host=db5000812537.hosting-data.io;database=dbs721306;username=dbu662955;password=1RdF5ghG17Cp!;port=3306");
        }
    }
}