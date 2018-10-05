using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ErisHub.Database.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ErisHub.Core.Players
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayersController : ControllerBase
    {
        private readonly ErisContext _db;
        private readonly IMapper _mapper;

        public PlayersController(ErisContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        // GET: api/players
        [HttpGet]
        public IActionResult GetPlayers()
        {
            var players = _db.Players.Select(player => _mapper.Map<PlayerDto>(player));
            return Ok(players);
        }

        // GET: api/players/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPlayers([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var player = await _db.Players
                .SingleOrDefaultAsync(x => x.Id == id);

            if (player == null)
            {
                return NotFound();
            }


            return Ok(_mapper.Map<PlayerDto>(player));
        }
    }
}