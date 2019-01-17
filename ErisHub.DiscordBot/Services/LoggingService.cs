using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;

namespace ErisHub.DiscordBot.Services
{

    // taken from patek bot
    public class LoggingService
    {
        private readonly BaseSocketClient _discord;
        private readonly CommandService _commands;

        private ILoggerFactory _loggerFactory;
        private ILogger _discordLogger;
        private ILogger _commandsLogger;

        public LoggingService(BaseSocketClient discord, CommandService commands, ILoggerFactory loggerFactory)
        {
            _discord = discord;
            _commands = commands;
            _loggerFactory = loggerFactory;
        }

        public void Init()
        {
            _discordLogger = _loggerFactory.CreateLogger("Discord");
            _commandsLogger = _loggerFactory.CreateLogger("Commands");

            _discord.Log += LogDiscord;
            _commands.Log += LogCommand;
        }

        private Task LogCommand(LogMessage message)
        {
            if (message.Exception is CommandException commandException)
            {
                var _ = commandException.Context.Channel.SendMessageAsync($"Error: {commandException.Message}");
            }

            _commandsLogger.Log(
                LogLevelFromSeverity(message.Severity),
                0,
                message,
                message.Exception,
                delegate { return message.ToString(prependTimestamp: false); }
            );

            return Task.CompletedTask;
        }

        private Task LogDiscord(LogMessage message)
        {
            _discordLogger.Log(
                LogLevelFromSeverity(message.Severity),
                0,
                message,
                message.Exception,
                delegate { return message.ToString(prependTimestamp: false); });
            return Task.CompletedTask;
        }

        private static LogLevel LogLevelFromSeverity(LogSeverity severity)
            => (LogLevel)(Math.Abs((int)severity - 5));
    }
}
