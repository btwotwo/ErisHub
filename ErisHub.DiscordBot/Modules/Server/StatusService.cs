using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using ErisHub.DiscordBot.ApiClient;
using ErisHub.DiscordBot.Util.Timer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ErisHub.DiscordBot.Modules.Server
{
    public class StatusService
    {
        private readonly IWaitingTimer _waitingTimer;

        private IUserMessage _statusMessage;
        private IEnumerable<StatusModel> _statuses;
        private CancellationTokenSource _tokenSource;
        private readonly HashSet<string> _hidden;

        private readonly BaseSocketClient _discord;
        private readonly ILogger _logger;
        private readonly IConfiguration _config;
        private readonly ServersClient _api;


        public StatusService(ServersClient api, BaseSocketClient discord, IConfiguration config,
            ILoggerFactory loggerFactory, IWaitingTimer waitingTimer)
        {
            _api = api;
            _config = config;
            _discord = discord;
            _logger = loggerFactory.CreateLogger("Status updater");
            _hidden = new HashSet<string>();
            _waitingTimer = waitingTimer;
        }

        public async Task StartAsync()
        {
            if (_statusMessage == null)
            {
                await UpdateStatuses();

                var channel = (IMessageChannel)_discord.GetChannel(_config.GetValue<ulong>("StatusChannelId"));
                _statusMessage = await channel.SendMessageAsync("", false, GenerateEmbed());
            }

            if (_waitingTimer.IsRunning())
            {
                throw new StatusException("Already started");
            }

            _tokenSource = new CancellationTokenSource();
            _waitingTimer.RunPeriodically(UpdateAsync, TimeSpan.FromSeconds(10), _tokenSource.Token);
        }

        public async Task StopAsync()
        {
            if (_waitingTimer.IsRunning() == false)
            {
                throw new StatusException("Already stopped.");
            }

            _tokenSource.Cancel();
            await _statusMessage.DeleteAsync();
        }

        public void Hide(string serverName)
        {
            if (_hidden.Contains(serverName))
            {
                throw new StatusException("Already hidden.");
            }

            if (_statuses.Any(x => x.Name == serverName) )
            {
                _hidden.Add(serverName);
            }
            else
            {
                throw new StatusException("Not valid name.");
            }
        }

        public void Unhide(string serverName)
        {
            if (_statuses.Any(x => x.Name == serverName))
            {
                if (_hidden.Contains(serverName))
                {
                    _hidden.Remove(serverName);
                }
                else
                {
                    throw new StatusException("Not hidden.");
                }
            }
            else
            {
                throw new StatusException("Not valid name.");
            }
        }

        private async Task UpdateAsync()
        {
            try
            {
                await UpdateStatuses();
                await _statusMessage.ModifyAsync(x => x.Embed = GenerateEmbed());
            }
            catch (Exception e)
            {
                _logger.LogError(default(EventId), e, "Error while updating message");
            }
        }


        private Embed GenerateEmbed()
        {
            var builder = new EmbedBuilder();
            builder.WithColor(Color.Green);

            foreach (var status in _statuses)
            {
                if (_hidden.Contains(status.Name))
                {
                    continue;
                }

                if (status.Online)
                {
                    builder.AddField(status.Name, $"<{status.Address}>");
                    builder.AddField("Players", status.Players, inline: true);
                    builder.AddField("Admins", status.Admins, inline: true);
                    builder.AddField("Round duration", status.RoundDuration);
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

        private async Task UpdateStatuses()
        {
            try
            {
                var statuses = await _api.GetStatusesAsync();

                _statuses = statuses.OrderBy(x => x.Name);
            }
            catch (Exception e)
            {
                _logger.LogError(default(EventId), e, "Failed to retrieve server list.");
            }
        }
    }

    public class StatusException : Exception
    {
        public StatusException(string msg) : base(msg) { }
    }
}
