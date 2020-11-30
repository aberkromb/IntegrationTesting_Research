using System;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using IntegrationTestingSandbox.DataAccess;
using IntegrationTestingSandbox.DataAccess.DataBase;
using IntegrationTestingSandbox.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Http;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace IntegrationTestingSandbox
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
            services.AddControllers();

            services.AddHealthChecks().AddDbContextCheck<PostgresDbContext>();
            services.Configure<PostgresOptions>(Configuration.GetSection(nameof(PostgresOptions)));

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "IntegrationTestingSandbox", Version = "v1"});
            });

            services.AddDbContext<PostgresDbContext>();
            services.AddTransient<IDataAccess, PostgresDataAccess>();

            services.AddHttpClient("google");
            services.AddHttpClient(string.Empty);
            
            
            // services.AddTransient<HttpMessageHandlerBuilder>(provider =>
            // {
            //     var builder = provider.GetRequiredService<HttpMessageHandlerBuilder>();
            //     return builder;
            // });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "IntegrationTestingSandbox v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health");
            });
        }
    }
    
    
}