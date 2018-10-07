using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ErisHub.Database.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Controller = ErisHub.Helpers.Controller;

namespace ErisHub.Core.Players
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayersController : Controller
    {
        private readonly ErisContext _db;
        private readonly IMapper _mapper;

        public PlayersController(ErisContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetAll([FromQuery] string name)
        {
            var players = _db.Players.Select(player => _mapper.Map<PlayerDto>(player));
            if (name != null)
            {
                players = players.Where(x => x.Ckey == name);
            }
            return Ok(players);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOne(int id)
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