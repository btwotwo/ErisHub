using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ErisHub.Database.Models;

namespace ErisHub.Core.Bans
{
    [Route("api/[controller]")]
    [ApiController]
    public class BansController : ControllerBase
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