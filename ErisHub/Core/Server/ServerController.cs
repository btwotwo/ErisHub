using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ErisHub.Core.Server.Models;
using ErisHub.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Controller = ErisHub.Helpers.Controller;

namespace ErisHub.Core.Server
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServerController : Controller
    {
        private readonly ServerStore _servers;

        public ServerController(ServerStore servers)
        {
            _servers = servers;
        }

        [HttpGet("{serverName}/config")]
        public IActionResult GetConfigs(string serverName)
        {
            var server = _servers.GetServer(serverName);

            if (server == null)
            {
                return BadRequest($"{serverName} not found");
            }

            var configPath = Path.Combine(server.Path, server.ConfigDir);
            var configs = GetConfigFileNames(configPath).Where(x => x != ".gitignore");

            return Ok(configs);
        }

        [HttpGet("{serverName}/config/{configName}")]
        public async Task<IActionResult> GetConfigAsync(string serverName, string configName)
        {
            var server = _servers.GetServer(serverName);

            if (server == null)
            {
                return BadRequest($"{serverName} not found");
            }

            var configPath = Path.Combine(server.Path, server.ConfigDir);
            var configFileName = GetConfigFileNames(configPath).SingleOrDefault(x => x == configName);

            if (configFileName == null)
            {
                return BadRequest($"{configName} not found.");
            }

            using (var configFile = System.IO.File.OpenText(Path.Combine(configPath, configFileName)))
            {
                var configFileText = await configFile.ReadToEndAsync();

                var result = new ServerConfig()
                {
                    Contents = configFileText,
                    Filename = configFileName
                };

                return Ok(result);
            }
        }

        [HttpPost("{serverName}/config/{configName}")]
        public async Task<IActionResult> UpdateConfigAsync(string serverName, string configName,
            [FromBody] ServerConfig updated)
        {
            var server = _servers.GetServer(serverName);

            if (server == null)
            {
                return BadRequest($"{serverName} not found.");
            }

            var configPath = Path.Combine(server.Path, server.ConfigDir);
            var configFileName = GetConfigFileNames(configPath).SingleOrDefault(x => x == configName);

            if (configFileName == null)
            {
                return BadRequest($"{configName} not found.");
            }

            await System.IO.File.WriteAllTextAsync(Path.Combine(configPath, configFileName), updated.Contents);

            return Ok();
        }

        private static IEnumerable<string> GetConfigFileNames(string configPath)
        {
            return Directory.EnumerateFiles(configPath).Select(Path.GetFileName);
        }
    }
}