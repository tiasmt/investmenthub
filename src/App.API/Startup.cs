using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.API.Services;
using App.DataLayer;
using App.DataLayer.Repository;
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

namespace investmenthub
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
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "investmenthub", Version = "v1" });
            });
            services.AddScoped<IPortfolioService, PortfolioService>();
            services.AddScoped<IRepository, SQLRepository>();
            // services.AddDbContext<InvestmentHubContext>(
            //         options =>
            //             options.UseSqlServer(
            //                 Configuration.GetConnectionString("DefaultConnection"),
            //                 x => x.MigrationsAssembly("App.DataLayer")));
            services.AddDbContext<InvestmentHubContext>(
                    options =>
                        options.UseInMemoryDatabase("InMemory"), ServiceLifetime.Singleton, ServiceLifetime.Singleton);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "investmenthub v1"));
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
