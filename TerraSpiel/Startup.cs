using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TerraSpiel.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TerraSpiel.Bot.Services;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace TerraSpiel.Bot
{
    public class Startup
    {
        //Comandos EF:
        //dotnet-ef migrations add InitialCreate -p ../TerraSpiel.DAL.Migrations/TerraSpiel.DAL.Migrations.csproj --context TerraSpiel.DAL.GameContext (pararse en la carpeta del bot)
        //dotnet-ef database update -p ../TerraSpiel.DAL.Migrations/TerraSpiel.DAL.Migrations.csproj --context TerraSpiel.DAL.GameContext
        public void ConfigureServices(IServiceCollection services)
        {
            var json = string.Empty;

            using (var fs = File.OpenRead("Json/config.json"))
            using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                json = sr.ReadToEnd();

            var configJson = JsonConvert.DeserializeObject<ConfigJson>(json);

            var gameConfig = new GameConfiguration(configJson);
            services.AddSingleton(gameConfig);

            services.AddDbContext<GameContext>(options =>
            {
                options.UseMySQL(configJson.ConnectionString,
                    x => x.MigrationsAssembly("TerraSpiel.DAL.Migrations"));
                //options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            });

            services.AddScoped<IGameService, GameService>();

            var serviceProvider = services.BuildServiceProvider();


            var bot = new Bot(serviceProvider);
            services.AddSingleton(bot);
        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

        }
    }
}
