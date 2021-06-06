using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ErisHub.DiscordBot.Modules.Kotmap
{
    [Group("kotmap")]
    [RequireUserPermission(GuildPermission.Administrator)]
    public class KotmapModule : ModuleBase
    {
        private readonly BaseSocketClient _discord;
        private readonly ulong _kotmapChannelId;

        public KotmapModule(IConfiguration configuration, BaseSocketClient discord)
        {
            _discord = discord;

            var kotmapChannelId = configuration.GetValue<ulong>("KotmapChannelId");

            if (kotmapChannelId == default)
            {
                throw new ArgumentNullException("KotmapChannelId is not specified in config");
            }

            _kotmapChannelId = kotmapChannelId;
        }

        [Command("add")]
        [Summary("Adds message to the kotmap channel (specified in config)")]
        public async Task AddMessageAsync(string message)
        {
            var channel = GetKotmapChannel();
            var discordMessage = await channel.SendMessageAsync(message);

            await discordMessage.ModifyAsync(msg => msg.Content = EnrichMessageWithId(message, discordMessage.Id));

            await ReplyAsync("Message sent.");
        }

        [Command("change")]
        [Summary("Edits the specified message in the kotmap channel (specifed in config)")]
        public async Task EditMessageAsync(ulong messageId, string contents)
        {
            var channel = GetKotmapChannel();
            var discordMessage = await channel.GetMessageAsync(messageId);

            if (!(discordMessage is IUserMessage userMessage))
            {
                await ReplyAsync("Message not found");
                return;
            }

            contents = EnrichMessageWithId(contents, messageId);

            await userMessage.ModifyAsync(msg => msg.Content = contents);
            await ReplyAsync("Message edited");
        }

        string EnrichMessageWithId(string originalMessage, ulong messageId) => originalMessage + $"\n`id: {messageId}`";

        IMessageChannel GetKotmapChannel() => (IMessageChannel)_discord.GetChannel(_kotmapChannelId);
    } 
}
