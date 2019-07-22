using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ErisHub.Database.Models;
using ErisHub.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ErisHub.Core.Players
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayersApiController : ApiController
    {
        private readonly ErisContext _db;
        private readonly IMapper _mapper;

        public PlayersApiController(ErisContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        [HttpGet("{name}")]
        public ActionResult<List<PlayerDto>> GetAll(string name)
        {
            var players = _db.Players.Select(player => _mapper.Map<PlayerDto>(player));
            if (name != null)
            {
                players = players.Where(x => x.Ckey == name);
            }
            return players.ToList();
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PlayerDto>> GetOne(int id)
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


            return _mapper.Map<PlayerDto>(player);
        }
    }
}