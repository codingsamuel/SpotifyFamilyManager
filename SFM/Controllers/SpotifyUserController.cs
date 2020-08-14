using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        public async Task<ActionResult<List<SpotifyUser>>> Get()
        {
            return Ok(await _context.SpotifyUsers.ToListAsync());
        }
        
        [HttpPost]
        public async Task<ActionResult<SpotifyUser>> Post([FromBody] SpotifyUser spotifyUser)
        {
            try
            {
                await _context.SpotifyUsers.AddAsync(spotifyUser);
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}