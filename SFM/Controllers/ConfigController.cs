using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SFM.Models;
using SFM.Models.ViewModels;

namespace SFM.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConfigController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        private readonly IMapper _mapper;

        public ConfigController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<SpotifyConfigViewModel>> GetSpotifyConfig()
        {
            try
            {
                var dbConfig = await _context.Configs
                    .Where(c =>
                        new[] {Config.SPOTIFY_CLIENT_ID, Config.SPOTIFY_CLIENT_SECRET}.Contains(c.Key))
                    .ToListAsync();

                return Ok(new SpotifyConfigViewModel
                {
                    ClientId = dbConfig.FirstOrDefault(c => c.Key == Config.SPOTIFY_CLIENT_ID)?.Value,
                    ClientSecret = dbConfig.FirstOrDefault(c => c.Key == Config.SPOTIFY_CLIENT_SECRET)?.Value
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<string>> AddOrUpdate([FromBody] ConfigViewModel config)
        {
            try
            {
                var dbConfig = await _context.Configs.FirstOrDefaultAsync(c => c.Key == config.Key);
                var mappedConfig = _mapper.Map<Config>(config);

                if (dbConfig == null)
                {
                    await _context.Configs.AddAsync(mappedConfig);
                }
                else
                {
                    mappedConfig.Id = dbConfig.Id;
                    _context.Configs.Update(mappedConfig);
                }

                await _context.SaveChangesAsync();
                return Ok("Done!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch("{oldKey}/[action]/{newKey}")]
        public async Task<ActionResult<string>> RenameKey([FromRoute] string oldKey, [FromRoute] string newKey)
        {
            try
            {
                var dbConfig = await _context.Configs.FirstOrDefaultAsync(c => c.Key == oldKey);

                if (dbConfig == null)
                    return NotFound();

                dbConfig.Key = newKey;

                await _context.SaveChangesAsync();

                return Ok($"Renamed Key '{oldKey}' to '{newKey}'");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}