using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ncahe_dotnet.Managers;
using NLog;
using NLog.Extensions.Logging;

namespace ncahe_dotnet
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
            LogManager.LoadConfiguration("nlog.xml");

            services.AddControllers();
            services.AddHttpClient();

            services.AddLogging(l => {
                l.ClearProviders();
                l.AddNLog();
            }).Configure<LoggerFilterOptions>(c => c.MinLevel = Microsoft.Extensions.Logging.LogLevel.Trace);

            services.AddDistributedRedisCache(option =>
            {
                option.Configuration = Environment.GetEnvironmentVariable("REDIS_HOST") != null ?  
                    Environment.GetEnvironmentVariable("REDIS_HOST") :
                    "127.0.0.1";
                option.InstanceName = "master";
            });

            services.AddSingleton(typeof(ICacheManager), typeof(CacheManager));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
