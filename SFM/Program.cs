using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using SFM.Models;

namespace SFM
{
    public class Program
    {
        public static void Main(string[] args)
        {
            using var context = new ApplicationDbContext();
            context.Database.EnsureCreated();
            
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
}