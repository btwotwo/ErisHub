using Discord;
using ErisHub.DiscordBot.ApiClient;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ErisHub.DiscordBot.Modules.Server
{
    public class StatusMessage : IStatusMessage
    {
        private readonly ulong _channelId;
        private readonly IDiscordClient _discordClient;

        private IUserMessage _message;

        public StatusMessage(ulong channelId, IDiscordClient discordClient)
        {
            _channelId = channelId;
            _discordClient = discordClient;
        }

        public async Task<ulong> TryCreateFromIdAsync(ulong id)
        {
            var channel = await _discordClient.GetChannelAsync(_channelId);
            if (channel is not IMessageChannel messageChannel)
            {
                throw new InvalidChannelStatusException(_channelId);
            }

            var message = id switch
            {
                0 => null,
                _ => await messageChannel.GetMessageAsync(id) as IUserMessage
            } ?? await messageChannel.SendMessageAsync("Updating...");

            _message = message;

            return _message.Id;
        }

        public async Task UpdateAsync(IEnumerable<StatusModel> statuses)
        {
            if (statuses.Any())
            {
                var embed = GenerateEmbed(statuses);
                await _message.ModifyAsync(msg =>
                {
                    msg.Embed = embed;
                    msg.Content = "";
                });
                return;
            }

            await _message.ModifyAsync(msg => msg.Content = "Server list is empty.");
        }

        private static Embed GenerateEmbed(IEnumerable<StatusModel> statuses)
        {
            var builder = new EmbedBuilder();
            builder.WithColor(Color.Green);

            foreach (var status in statuses)
            {
                if (status.Online)
                {
                    builder.AddField(status.Name, $"<{status.Address}>")
                           .AddField("Players", status.Players, inline: true)
                           .AddField("Admins", status.Admins, inline: true)
                           .AddField("Round duration", status.RoundDuration);
                }
                else
                {
                    builder.AddField(status.Name, "Server is offline");
                }

                builder.AddField("\u200b", "\u200b"); // to emulate a new line
            }
            builder.Fields.RemoveAt(builder.Fields.Count - 1); //to remove new line from the last line
            return builder.Build();
        }

    }
}
