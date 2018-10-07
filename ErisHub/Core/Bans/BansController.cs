using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ErisHub.Database.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Controller = ErisHub.Helpers.Controller;

namespace ErisHub.Core.Bans
{
    [Route("api/[controller]")]
    [ApiController]
    public class BansController : Controller
    {
        private readonly ErisContext _context;
        private readonly IMapper _mapper;

        public BansController(ErisContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var bans = await _context.Bans
                .Include(x => x.BannedBy)
                .Include(x => x.Target)
                .Include(x => x.UnbannedBy).ToListAsync();

            var result = bans.Select(ToDto);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var ban = await _context.Bans
                .Include(x => x.BannedBy)
                .Include(x => x.Target)
                .Include(x => x.UnbannedBy)
                .SingleOrDefaultAsync(x => x.Id == id);

            if (ban == null)
            {
                return NotFound();
            }

            return Ok(ToDto(ban));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] BanDto editBan)
        {
            var ban = await _context.Bans.SingleOrDefaultAsync(x => x.Id == id);

            if (ban == null)
            {
                return NotFound();
            }

            ban.Duration = editBan.Duration;

            return Ok();
        }

        private BanDto ToDto(Ban ban)
        {
            var dto = _mapper.Map<BanDto>(ban);
            dto.BannedByCkey = ban.BannedBy?.Ckey;
            dto.UnbannedByCkey = ban.UnbannedBy?.Ckey;
            dto.TargetCkey = ban.Target?.Ckey;
            return dto;
        }
    }
}