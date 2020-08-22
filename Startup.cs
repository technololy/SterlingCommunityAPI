using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using SterlingCommunityAPI.Models;
using SterlingCommunityAPI.Services;
using SterlingCommunityAPI.Services.Middleware;

namespace SterlingCommunityAPI
{
    public class Startup
    {
        private readonly ILogger<Startup> logger;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {


            services.AddControllers();

            var connection = Configuration.GetConnectionString("SterlingDigitalForumDb");

            //var connection = "Server=DESKTOP-JPCURN7\\SQLEXPRESS;Initial Catalog=SterlingCommunityDB;User Id=loladeking;Password=ishola1986";
            services.AddDbContext<SterlingCommunityDBContext>(options => options.UseSqlServer(connection));
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "SterlingCommunityAPI", Version = "v1" });
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {



            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
           // app.UseSwaggerUI(c =>
          //  {
            //    c.SwaggerEndpoint("/swagger/v1/swagger.json", "SterlingCommunityAPI V1");
            //});
            app.UseSwaggerUI(c =>
{
string swaggerJsonBasePath = string.IsNullOrWhiteSpace(c.RoutePrefix) ? "." : "..";
c.SwaggerEndpoint($"{swaggerJsonBasePath}/swagger/v1/swagger.json", "SterlingCommunityAPI V1");
});
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseMiddleware<EncryptDecrypt>();
            app.UseMiddleware<RequestResponseLoggingMiddleware>();
           // app.UseMiddleware<AuthorizationMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

        }
    }
}
