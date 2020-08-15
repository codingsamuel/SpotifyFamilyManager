using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PayPal;
using SFM.Models;

namespace SFM.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PayPalController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        
        public PayPalController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("{userId}/[action]")]
        public async Task<ActionResult> Subscribe([FromRoute] int userId)
        {
            try
            {
                var user = await _context.SpotifyUsers.FindAsync(userId);


                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}