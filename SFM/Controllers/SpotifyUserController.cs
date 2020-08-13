using System;
using System.Configuration;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using SFM.Models;

namespace SFM.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SpotifyUserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        
        public SpotifyUserController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult> Get()
        {
            
        }
        
        [HttpPost]
        public async Task<ActionResult<SpotifyUser>> Post([FromBody] SpotifyUser spotifyUser)
        {
            try
            {
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
    }
}