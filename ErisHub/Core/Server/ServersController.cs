using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ErisHub.Helpers;
using ErisHub.Shared;
using ErisHub.Shared.Server.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Caching.Memory;
using Controller = ErisHub.Helpers.Controller;

namespace ErisHub.Core.Server
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServersController : Controller
    {
        private readonly ServerStore _servers;
        private readonly IMemoryCache _cache;
        private const string StatusCommand = "status";

        public ServersController(ServerStore servers, IMemoryCache cache)
        {
            _servers = servers;
            _cache = cache;
        }

        [HttpGet]
        public async Task<IActionResult> GetServers()
        {
            var servers = _servers.GetAllServers();
            var statusesTasks = servers.Select(GetServerStatusAsync);

            var statuses = await Task.WhenAll(statusesTasks);

            return Ok(statuses);
        }

        [HttpGet("{id}/status")]
        public async Task<IActionResult> GetSingleServerStatusAsync(string id)
        {
            var server = _servers.GetServer(id);

            if (server == null)
            {
                return NotFound();
            }

            return Ok(await GetServerStatusAsync(server));

        }

        [HttpGet("{id}/config")]
        public IActionResult GetConfigNames(string id)
        {
            var server = _servers.GetServer(id);

            if (server == null)
            {
                return NotFound();
            }

            var configs = GetConfigFileNames(server.ConfigPath).Where(x => x != ".gitignore");

            return Ok(configs);
        }

        [HttpGet("{id}/config/{configName}")]
        public async Task<IActionResult> GetConfigAsync(string id, string configName)
        {
            var server = _servers.GetServer(id);

            if (server == null)
            {
                return NotFound();
            }

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

        [HttpPost("{id}/config/{configName}")]
        public async Task<IActionResult> UpdateConfigAsync(string id, string configName,
            [FromBody] string newContents)
        {
            var server = _servers.GetServer(id);

            if (server == null)
            {
                return NotFound();
            }

            var configFileName = GetConfigFileNames(server.ConfigPath).SingleOrDefault(x => x == configName);

            if (configFileName == null)
            {
                return BadRequest($"{configName} not found.");
            }

            await System.IO.File.WriteAllTextAsync(Path.Combine(server.ConfigPath, configFileName), newContents);

            return Ok();
        }

        private async Task<StatusModel> GetServerStatusAsync(Configuration.Server server)
        {
            if (_cache.TryGetValue(server.Id, out StatusModel model))
            {
                return model;
            }

            model = new StatusModel()
            {
                Name = server.Name,
                Address = $"byond://{server.Host}:{server.Port}"
            };
            var response =
                await ByondTopic.SendTopicCommandAsync(server.Host, server.Port.ToString(), StatusCommand);

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

            _cache.Set(server.Id, model, TimeSpan.FromSeconds(5));

            return model;
        }

        private static IEnumerable<string> GetConfigFileNames(string configPath)
        {
            return Directory.EnumerateFiles(configPath).Select(Path.GetFileName);
        }
    }
}