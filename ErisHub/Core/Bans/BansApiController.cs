using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ErisHub.Database.Models;
using ErisHub.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ErisHub.Core.Bans
{
    [Route("api/[controller]")]
    [ApiController]
    public class BansApiController : ApiController
    {
        private readonly ErisContext _context;
        private readonly IMapper _mapper;

        public BansApiController(ErisContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<BanDto>>> Get()
        {
            var bans = await _context.Bans.AsQueryable()
                .Include(x => x.BannedBy)
                .Include(x => x.Target)
                .Include(x => x.UnbannedBy).ToListAsync();

            var result = bans.Select(ToDto).ToList();
            return result;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BanDto>> Get(int id)
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

            return ToDto(ban);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] BanDto editBan)
        {
            var ban = await _context.Bans.AsQueryable().SingleOrDefaultAsync(x => x.Id == id);

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