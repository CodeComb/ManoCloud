using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Data.Entity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Mano.Models;

namespace Mano
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddEntityFramework()
                .AddDbContext<ManoContext>(x => x.UseNpgsql("User ID=postgres;Password=123456;Host=localhost;Port=5432;Database=mano;"))
                .AddNpgsql();

            services.AddIdentity<User, IdentityRole<long>>(x =>
            {
                x.Password.RequireDigit = false;
                x.Password.RequiredLength = 0;
                x.Password.RequireLowercase = false;
                x.Password.RequireNonLetterOrDigit = false;
                x.Password.RequireUppercase = false;
                x.User.AllowedUserNameCharacters = null;
            })
                      .AddEntityFrameworkStores<ManoContext, long>()
                      .AddDefaultTokenProviders();

            services.AddMvc()
                .AddTemplate()
                .AddCookieTemplateProvider();
            
            services.AddFileUpload()
                .AddEntityFrameworkStorage<ManoContext>();

            services.AddSmartUser<User, long>();
            services.AddSmartCookies();
            services.AddSignalR();
            services.AddAntiXss();
            services.AddConfiguration();
            services.AddSmtpEmailSender("smtp.exmail.qq.com", 25, "Mano Cloud", "noreply@mano.cloud", "noreply@mano.cloud", "ManoCloud123456");
            services.AddAesCrypto();
        }

        public async void Configure(IApplicationBuilder app, ILoggerFactory logger)
        {
            logger.AddConsole();
            logger.MinimumLevel = LogLevel.Warning;

            app.UseFileUpload("/assets/shared/jquery.codecomb.fileupload.js");
            app.UseAutoAjax();
            app.UseIdentity();
            app.UseStaticFiles();
            app.UseSignalR();
            app.UseMvcWithDefaultRoute();

            await SampleData.InitDB(app.ApplicationServices);
        }

        public static void Main(string[] args) => WebApplication.Run<Startup>(args);
    }
}
