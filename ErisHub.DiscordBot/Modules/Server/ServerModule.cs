using Discord.Commands;
using System;
using System.Threading.Tasks;

namespace ErisHub.DiscordBot.Modules.Server
{
    [Group("status")]
    [RequireUserPermission(Discord.GuildPermission.Administrator)]
    public class StatusModule : ModuleBase
    {
        private readonly IStatusUpdater _statusUpdater;
        private readonly IStatusHider _statusHider;
        private readonly IStatusProvider _statusProvider;

        public StatusModule(IStatusUpdater statusUpdater, IStatusHider statusHider, IStatusProvider statusProvider)
        {
            _statusUpdater = statusUpdater;
            _statusHider = statusHider;
            _statusProvider = statusProvider;
        }

        [Command("start"), Summary("Starts status updating.")]
        public async Task StartStatusUpdateAsync()
        {
            try
            {
                await ReplyAsync("Starting...");
                var result = await _statusUpdater.StartAsync();
                if (result != null)
                {
                    await ReplyAsync(result.Message);
                }
                await ReplyAsync("Started.");
            }
            catch (Exception e)
            {
                await ReplyAsync(e.Message);
            }
        }

        [Command("stop")]
        public async Task StopStatusUpdateAsync()
        {
            try
            {
                await ReplyAsync("Why would you ever stop it? Anyway, stopping...");
                var result = await _statusUpdater.StopAsync();
                if (result != null)
                {
                    await ReplyAsync(result.Message);
                }
                await ReplyAsync("Stopped.");
            }
            catch (StatusException e)
            {
                await ReplyAsync(e.Message);
            }
        }

        [Command("hide")]
        public async Task HideServer(string name)
        {
            try
            {
                var status = await _statusProvider.GetByNameOrDefaultAsync(name);

                if (status == null)
                {
                    await ReplyAsync("Invalid server name!");
                    return;
                }

                if (_statusHider.Hidden(status))
                {
                    await ReplyAsync("Already hidden");
                }

                _statusHider.Hide(status);
                await ReplyAsync("Hidden.");
            }
            catch (StatusException e)
            {
                await ReplyAsync(e.Message);
            }
        }

        [Command("show")]
        public async Task ShowServer(string name)
        {
            try
            {
                var status = await _statusProvider.GetByNameOrDefaultAsync(name);

                if (status == null)
                {
                    await ReplyAsync("Invalid server name!");
                    return;
                }

                if (!_statusHider.Hidden(status))
                {
                    await ReplyAsync("Not hidden");
                }

                _statusHider.Show(status);
                await ReplyAsync("Unhidden.");
            }
            catch (StatusException e)
            {
                await ReplyAsync(e.Message);
            }
        }
    }
}
