using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ErisHub.Helpers;
using ErisHub.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Caching.Memory;

namespace ErisHub.Core.Server
{
    public class ServersApiController : ApiController
    {
        private readonly ServerStore _servers;
        private readonly IMemoryCache _cache;
        private const string StatusCommand = "status";

        public ServersApiController(ServerStore servers, IMemoryCache cache)
        {
            _servers = servers;
            _cache = cache;
        }

        [HttpGet]
        public async Task<ActionResult<StatusModel[]>> GetServers()
        {
            var servers = _servers.GetAllServers();
            var statusesTasks = servers.Select(GetServerStatusAsync);

            var statuses = await Task.WhenAll(statusesTasks);

            return statuses;
        }

        [HttpGet("{id}/status")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<StatusModel>> GetSingleServerStatusAsync(string id)
        {
            var server = _servers.GetServer(id);

            if (server == null)
            {
                return NotFound();
            }

            var status = await GetServerStatusAsync(server);
            return status;

        }

        [HttpGet("{id}/config")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<string>> GetConfigNames(string id)
        {
            var server = _servers.GetServer(id);

            if (server == null)
            {
                return NotFound();
            }

            var configs = GetConfigFileNames(server.ConfigPath).Where(x => x != ".gitignore");

            return configs.ToList();
        }

        [HttpGet("{id}/config/{configName}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<string>> GetConfigAsync(string id, string configName)
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
                return configFileText;
            }

        }

        [HttpPost("{id}/config/{configName}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> UpdateConfigAsync(string id, string configName,
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

            if (response == null || string.IsNullOrWhiteSpace(response))
            {
                model.Online = false;
            }
            else
            {
                var parsedResponse = QueryHelpers.ParseQuery(response);
                model.Players = int.Parse(parsedResponse["players"]);
                model.Admins = int.Parse(parsedResponse["admins"]);
                model.RoundDuration = parsedResponse["roundduration"];
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