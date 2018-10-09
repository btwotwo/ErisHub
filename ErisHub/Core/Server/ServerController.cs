using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ErisHub.Core.Server.Models;
using ErisHub.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Caching.Memory;
using Controller = ErisHub.Helpers.Controller;

namespace ErisHub.Core.Server
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServerController : Controller
    {
        private readonly ServerStore _servers;
        private readonly IMemoryCache _cache;
        private const string StatusCommand = "status";

        public ServerController(ServerStore servers, IMemoryCache cache)
        {
            _servers = servers;
            _cache = cache;
        }

        [HttpGet]
        public IActionResult GetServers()
        {
            return Ok(_servers.GetAllServers());
        }

        [HttpGet("{serverName}/status")]
        public async Task<IActionResult> GetServerStatusAsync(string serverName)
        {
            try
            {
                var server = TryGetServer(serverName);

                if (_cache.TryGetValue(server.Name, out StatusModel model))
                {
                    return Ok(model);
                }

                model = new StatusModel()
                {
                    Name = serverName,
                    Address = $"byond://{server.Host}:{server.Port}"
                };
                var response = await ByondTopic.SendTopicCommandAsync(server.Host, server.Port.ToString(), StatusCommand);

                if (response == null)
                {
                    model.Online = false;
                }
                else
                {
                    var parsedResponse = QueryHelpers.ParseQuery(response);
                    model.Players = int.Parse(parsedResponse["players"]);
                    model.Admins = int.Parse(parsedResponse["admins"]);
                    model.Online = true;
                }

                _cache.Set(server.Name, model, TimeSpan.FromSeconds(5));

                return Ok(model);


            }
            catch (ServerNameInvalidException e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("{serverName}/config")]
        public IActionResult GetConfigNames(string serverName)
        {
            try
            {
                var server = TryGetServer(serverName);
                var configs = GetConfigFileNames(server.ConfigPath).Where(x => x != ".gitignore");

                return Ok(configs);
            }
            catch (ServerNameInvalidException e)
            {
                return BadRequest(e.Message);
            }


        }

        [HttpGet("{serverName}/config/{configName}")]
        public async Task<IActionResult> GetConfigAsync(string serverName, string configName)
        {
            try
            {
                var server = TryGetServer(serverName);

                var configFileName = GetConfigFileNames(server.ConfigPath).SingleOrDefault(x => x == configName);

                if (configFileName == null)
                {
                    return BadRequest($"{configName} not found.");
                }

                using (var configFile = System.IO.File.OpenText(Path.Combine(server.ConfigPath, configFileName)))
                {
                    var configFileText = await configFile.ReadToEndAsync();
                    return Ok(configFileText);
                }
            }
            catch (ServerNameInvalidException e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("{serverName}/config/{configName}")]
        public async Task<IActionResult> UpdateConfigAsync(string serverName, string configName,
            [FromBody] string newContents)
        {
            try
            {
                var server = TryGetServer(serverName);

                var configFileName = GetConfigFileNames(server.ConfigPath).SingleOrDefault(x => x == configName);

                if (configFileName == null)
                {
                    return BadRequest($"{configName} not found.");
                }

                await System.IO.File.WriteAllTextAsync(Path.Combine(server.ConfigPath, configFileName), newContents);

                return Ok();
            }
            catch (ServerNameInvalidException e)
            {
                return BadRequest(e.Message);
            }
        }

        private static IEnumerable<string> GetConfigFileNames(string configPath)
        {
            return Directory.EnumerateFiles(configPath).Select(Path.GetFileName);
        }

        private Configuration.Server TryGetServer(string serverName)
        {
            var server = _servers.GetServer(serverName);

            if (server == null)
            {
                throw new ServerNameInvalidException(serverName);
            }

            return server;
        }

        private class ServerNameInvalidException : Exception
        {
            public ServerNameInvalidException(string serverName) : base($"{serverName} not found")
            {
            }
        }
    }
}