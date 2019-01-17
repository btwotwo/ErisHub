using System.Threading.Tasks;
using Discord.Commands;

namespace ErisHub.DiscordBot.Modules.Server
{
    [Group("status")]
    public class StatusModule : ModuleBase
    {
        private readonly StatusService _status;
        private const string StatusStart = "start";
        private const string StatusStop = "stop";

        public StatusModule(StatusService status)
        {
            _status = status;
        }

        [Command("start"), Summary("Starts status updating.")]
        public async Task StartStatusUpdateAsync()
        {
            try
            {
                await ReplyAsync("Starting...");
                await _status.StartAsync();
                await ReplyAsync("Started.");
            }
            catch (StatusException e)
            {
                await ReplyAsync(e.Message);
            }
        }

        [Command("stop")]
        public async Task StopStatusUpdateAsync()
        {
            try
            {
                await ReplyAsync("Stopping...");
                await _status.StopAsync();
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
                _status.Hide(name);
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
                _status.Unhide(name);
                await ReplyAsync("Unhidden.");
            }
            catch (StatusException e)
            {
                await ReplyAsync(e.Message);
            }
        }
    }
}
