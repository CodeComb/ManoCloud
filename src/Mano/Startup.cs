using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.IO;
using System.Net.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Mano.Models;

namespace Mano
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ManoContext>(x => x.UseMySql("server=localhost;database=mano;uid=root;pwd=19931101;"));

            services.AddIdentity<User, IdentityRole<long>>(x =>
            {
                x.Password.RequireDigit = false;
                x.Password.RequiredLength = 0;
                x.Password.RequireLowercase = false;
                x.Password.RequireNonAlphanumeric = false;
                x.Password.RequireUppercase = false;
                x.User.AllowedUserNameCharacters = null;
            })
                      .AddEntityFrameworkStores<ManoContext, long>()
                      .AddDefaultTokenProviders();

            services.AddMvc()
                .AddMultiTemplateEngine()
                .AddCookieTemplateProvider();
            
            services.AddBlobStorage()
                .AddEntityFrameworkStorage<ManoContext>();

            services.AddSmartUser<User, long>();
            services.AddSmartCookies();
            services.AddSignalR();
            services.AddAntiXss();
            services.AddConfiguration();
            services.AddTimedJob();
            services.AddSmtpEmailSender("smtp.exmail.qq.com", 25, "Mano Cloud", "noreply@mano.cloud", "noreply@mano.cloud", "ManoCloud123456");
            services.AddAesCrypto();
        }

        public async void Configure(IApplicationBuilder app, ILoggerFactory logger)
        {
            logger.AddConsole();

            app.UseBlobStorage("/assets/shared/scripts/jquery.codecomb.fileupload.js");
            app.UseAutoAjax();
            app.UseIdentity();
            app.UseStaticFiles();
            app.UseSignalR();
            app.UseMvcWithDefaultRoute();

            await SampleData.InitDB(app.ApplicationServices);

            app.UseTimedJob();
        }

        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseUrls("http://*:80")
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}