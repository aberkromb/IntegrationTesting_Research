using IntegrationTestingSandbox.DataAccess.DataBase;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
            
            services.AddHealthChecks()
                .AddDbContextCheck<PostgresDbContext>();
            
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "IntegrationTestingSandbox", Version = "v1"});
            });
            
            // services.AddDbContext<PostgresDbContext>(options => 
            //     options.UseNpgsql("Host=localhost; Port=5432; Database=postgres; Username=postgres; Password=mystrongpassword"));
             services.AddDbContext<PostgresDbContext>();

            // Configuration["PostgresOptions"] = JsonConvert.SerializeObject(new PostgresOptions
            //     {ConnectionString = "Host=localhos"Host=127.0.0.200; Port=5432; Database=postgres; Username=postgres; Password=mystrongpassword"t; Port=5432; Database=postgres; Username=postgres; Password=mystrongpassword"});
            // services.Configure<PostgresOptions>(Configuration.GetSection(nameof(PostgresOptions)));
            // services.PostConfigure<PostgresOptions>(options =>
            //     options.ConnectionString =
            //         "Host=127.0.0.200; Port=5432; Database=postgres; Username=postgres; Password=mystrongpassword");
            //
            services.AddTransient<IDataAccess, PostgresDataAccess>();
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