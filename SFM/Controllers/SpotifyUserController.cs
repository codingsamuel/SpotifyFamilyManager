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

        [HttpGet("{userId}/[action]")]
        public async Task<ActionResult<DateTime>> NextPayment([FromRoute]long userId)
        {
            try
            {
                var dbUser = await _context.SpotifyUsers.FirstOrDefaultAsync(u => u.Id == userId);
                if (dbUser == null)
                    return NotFound();
                
                var dbSubscription = await _context.Subscriptions.FirstOrDefaultAsync(s => s.SpotifyUserId == userId);
                if (dbSubscription == null)
                    return NotFound();
                
                // Calculate last payment
                var date = DateTime.Now;
                var span = DateTime.Now - dbSubscription.LastPayment;
                // if (span.Days > dbSubscription.PaymentInterval)
                // {
                //     
                // }
                return Ok(date.Add(span));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<SpotifyUser>> Post([FromBody] SpotifyUser user)
        {
            try
            {
                var dbUser = await _context.SpotifyUsers.FirstOrDefaultAsync(u => u.SpotifyId == user.SpotifyId);
                if (dbUser == null)
                {
                    user.Created = DateTime.Now;
                    user.Updated = DateTime.Now;
                    await _context.SpotifyUsers.AddAsync(user);   
                }
                else
                {
                    user.Updated = DateTime.Now;
                    user.Id = dbUser.Id;
                    dbUser = user;
                }
                
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