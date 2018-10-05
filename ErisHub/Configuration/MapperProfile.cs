using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ErisHub.Core.Players;
using ErisHub.Database.Models;

namespace ErisHub.Configuration
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<Player, PlayerDto>();
        }
    }
}
