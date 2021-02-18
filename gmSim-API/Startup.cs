using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using gmSim_API.MongoDocuments;
using gmSim_API.ProcessServices;
using gmSim_API.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

namespace gmSim_API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(policy =>
            {
                policy.AddPolicy("CorsPolicy", opt => opt
                    .AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod());
            });
            
            services.Configure<GmSimDatabaseSettings>(
                Configuration.GetSection(nameof(GmSimDatabaseSettings)));
            services.AddSingleton<IGmSimDatabaseSettings>(sp =>
                sp.GetRequiredService<IOptions<GmSimDatabaseSettings>>().Value);
            services.AddSingleton<NewSeasonProcessService>();
            services.AddSingleton<RegularSeasonWeekProcessService>();
            services.AddSingleton<DivisionalFinalsProcessService>();
            services.AddSingleton<GameService>();
            services.AddSingleton<TeamsService>();
            services.AddSingleton<FixturesService>();
            services.AddSingleton<StandingsService>();
            services.AddSingleton<CoachesService>();
            services.AddSingleton<GeneralManagersService>();
            services.AddSingleton<PlayersService>();
            
            services.AddControllers();
            services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new OpenApiInfo {Title = "gmSim_API", Version = "v1"}); });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "gmSim_API v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            
            app.UseCors("CorsPolicy");

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}