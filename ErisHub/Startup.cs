using System.IO;
using AutoMapper;
using ErisHub.Configuration;
using ErisHub.Core.Webhook;
using ErisHub.Database.Models;
using ErisHub.Helpers;
using ErisHub.Helpers.Auth;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ErisHub
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IHostingEnvironment env)
        {
            Configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("serverconfig.json", optional: false)
                .AddJsonFile($"serverconfig.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .Build();
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddDbContext<ErisContext>(options =>
            {
                options.UseMySql(Configuration.GetConnectionString("Eris"));
            });

            var mapperConfig = new MapperConfiguration(config => config.AddProfile<MapperProfile>());
            var mapper = mapperConfig.CreateMapper();
            services.AddSingleton(mapper);
            services.AddMemoryCache();
            services.AddSingleton<ServerStore>();
            services.AddSingleton(Configuration);
            services.AddScoped<WebhookHub>();
            services.AddSignalR();
            services.AddOpenApiDocument();

            services.AddAuthentication()
                .AddScheme<ApiKeyOptions, ApiKeyHandler>(Schemes.ApiKeyScheme, opts => opts.ApiKey = Configuration["ApiKey"]);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
            //     app.UseHsts();
            }

            app.UseSignalR(routes => routes.MapHub<WebhookHub>("/webhookHub"));
            // app.UseHttpsRedirection();
            app.UseMvc();
            app.UseOpenApi();
        }
    }
}
