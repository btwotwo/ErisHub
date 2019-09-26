using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using ErisHub.DiscordBot.Database;
using ErisHub.DiscordBot.Database.Models.Newcomer;
using ErisHub.DiscordBot.Util;
using ErisHub.DiscordBot.Util.CachedDbEntity;
using ErisHub.DiscordBot.Util.CachedRepo;

namespace ErisHub.DiscordBot.Modules.Newcomer
{
    [Group("newcomer")]
    public class NewcomerModule : ModuleBase
    {
        private readonly Regex _emoteRegex = new Regex("<a?:(.+):([0-9]+)>");
        private readonly ICachedRepo<NewcomerSetting> _settingsRepo;
        private readonly ICachedDbEntity<NewcomerConfig> _config;

        public NewcomerModule(BotContext db, ICachedRepo<NewcomerSetting> settingsRepo,
            ICachedDbEntity<NewcomerConfig> config)
        {
            _settingsRepo = settingsRepo;
            _config = config;
        }

        [Command("channels")]
        public async Task ListNewcomerChannels()
        {
            var channels = _settingsRepo.Cache.Select(x => $"<#{x.ChannelId}> - id: {x.Id}").ToList();

            if (channels.Any())
            {
                await ReplyAsync(string.Join('\n', channels));
            }
            else
            {
                await ReplyAsync("No newcomer channels.");
            }

        }

        [Command("add")]
        public async Task SetNewcomerChannel(IChannel channel, ulong messageId, string emoteText, IRole role)
        {
            // <:emotename:123123132132>
            var emote = _emoteRegex.Split(emoteText);

            if (emote.Length == 1)
            {
                await ReplyAsync("Please use only custom emotes.");
                return;
            }

            var guildChannel = await Context.Guild.GetMessageChannelOrDefaultAsync(channel);

            if (guildChannel == null)
            {
                await ReplyAsync("Invalid channel.");
                return;
            }

            var message = await guildChannel.GetMessageAsync(messageId);

            if (message == null)
            {
                await ReplyAsync("Message not found.");
                return;
            }
            var emoteId = ulong.Parse(emote[2]);

            if (_settingsRepo.Cache.Any(x =>
                x.ChannelId == channel.Id && x.MessageId == messageId && x.EmoteId == emoteId))
            {
                await ReplyAsync("This channel is already set as newcomer channel");
                return;
            }

            var newcomerSetting = new NewcomerSetting()
            {
                ChannelId = channel.Id,
                EmoteId = emoteId,
                MessageId = messageId,
                RoleId = role.Id
            };

            await _settingsRepo.AddAsync(newcomerSetting);

            await ReplyAsync($"{channel.Name}, {messageId}, {emoteText}");
        }

        [Command("remove")]
        public async Task RemoveSetting(int id)
        {
            var item = _settingsRepo.Cache.SingleOrDefault(x => x.Id == id);

            if (item == null)
            {
                await ReplyAsync("Not found.");
                return;
            }

            await _settingsRepo.DeleteAsync(item);
            await ReplyAsync("Removed.");
        }

        [Command("role")]
        public async Task SetNewcomerRole(IRole role)
        {
            var config = _config.CachedValue;
            config.NewcomerRoleId = role.Id;

            await _config.UpdateAsync(config);

            await ReplyAsync("Role set.");
        }
    }
}
