using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord;

namespace ErisHub.DiscordBot.Util
{
    public static class IGuildExtensions
    {
        public static async Task<IMessageChannel> GetMessageChannelOrDefaultAsync(this IGuild guild, IChannel channel)
        {
            var gChannel = await guild.GetChannelAsync(channel.Id);

            if (gChannel is IMessageChannel gMessageChannel)
            {
                return gMessageChannel;
            }

            return null;
        }
    }
}
