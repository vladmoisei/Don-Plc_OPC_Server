using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Don_PlcDashboard_and_Reports.Data;
using Don_PlcDashboard_and_Reports.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Don_PlcDashboard_and_Reports
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
            // @"Server=172.16.4.165\SQLEXPRESS;Database=ConsumGazDatabase;
            services.AddDbContext<RaportareDbContext>(options =>
            options.UseSqlServer(@"Server=172.16.4.165\SQLEXPRESS;Database=Don_DashboardReports;User Id=user; Password=Calarasi81; MultipleActiveResultSets=true;"));


            services.AddRazorPages(); // Added for Razor Pages
            services.AddServerSideBlazor(); // Added for Blazor
            services.AddControllersWithViews();
            services.AddScoped<HttpClient>();
            services.AddSingleton<PlcService>(); // Added Plc service, startd directly because it is not derived
            services.AddSingleton<TimedService>(); // Background Timd service
            services.AddSingleton<StartAutBackgroundService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapBlazorHub(); // Added for blazor
            });
        }
    }
}
