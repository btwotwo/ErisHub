using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using ErisHub.DiscordBot.Database.Models;
using ErisHub.DiscordBot.Util.CachedDbEntity;
using ErisHub.DiscordBot.Util.CachedRepo;

namespace ErisHub.DiscordBot.Modules.Newcomer
{
    public class NewcomerHandler
    {
        private readonly BaseSocketClient _client;
        private readonly ICachedRepo<NewcomerSetting> _settingsRepo;
        private readonly ICachedDbEntity<NewcomerConfig> _config;

        public NewcomerHandler(BaseSocketClient client, ICachedRepo<NewcomerSetting> settingsRepo,
            ICachedDbEntity<NewcomerConfig> config)
        {
            _client = client;
            _settingsRepo = settingsRepo;
            _config = config;
        }

        public void Init()
        {
            _client.UserJoined += user => Task.Run(() => ClientOnUserJoined(user));
            _client.ReactionAdded += (a, b, c) => Task.Run(() => ProcessReactionAsync(a, b, c));
        }

        private async Task ClientOnUserJoined(IGuildUser user)
        {
            var config = _config.CachedValue;

            if (config.NewcomerRoleId == null)
            {
                await SendWelcomeMessage();
                return;
            }

            var role = user.Guild.Roles.SingleOrDefault(x => x.Id == config.NewcomerRoleId);

            if (role == null)
            {
                await SendWelcomeMessage();
                return;
            }

            await user.AddRoleAsync(role);

            async Task SendWelcomeMessage()
            {
                await user.SendMessageAsync($"Please DM admins to gain access to the {user.Guild.Name}");
            }
        }


        private async Task ProcessReactionAsync(Cacheable<IUserMessage, ulong> message, ISocketMessageChannel channel,
            SocketReaction reaction)
        {
            var config = _config.CachedValue;

            if (config.NewcomerRoleId == null)
            {
                return;
            }


            if (!ValidateUser(reaction.User.GetValueOrDefault(), config.NewcomerRoleId.Value, out var user))
            {
                return;
            }


            switch (reaction.Emote)
            {
                case Emoji _:
                    return;
                case Emote emote:
                {
                    var role = _settingsRepo.Cache.SingleOrDefault(x =>
                        x.EmoteId == emote.Id && x.ChannelId == channel.Id && x.MessageId == message.Id);

                    if (role == null)
                    {
                        return;
                    }

                    var guild = user.Guild;

                    var newcomerRole = guild.Roles.Single(x => x.Id == _config.CachedValue.NewcomerRoleId.Value);
                    var memberRole = guild.Roles.Single(x => x.Id == role.RoleId);

                    await user.AddRoleAsync(memberRole);
                    await user.RemoveRoleAsync(newcomerRole);

                    var msg = await message.GetOrDownloadAsync();
                    await msg.RemoveReactionAsync(emote, user);

                    break;
                }
            }

        }

        private static bool ValidateUser(IUser user, ulong newcomerRole, out IGuildUser convertedUser)
        {
            if (user.IsBot || user.IsWebhook)
            {
                convertedUser = null;
                return false;
            }

            if (!(user is IGuildUser guildUser))
            {
                convertedUser = null;
                return false;
            }

            var hasRole = guildUser.RoleIds.Any(x => x == newcomerRole);

            convertedUser = hasRole ? guildUser : null;

            return hasRole;
        }
    }
}
