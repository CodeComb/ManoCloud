using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;
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
        private Timer SyncTimer { get; set; }
        private Timer NodeTimer { get; set; }
        private Timer ProjectTimer { get; set; }

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

            app.UseFileUpload("/assets/shared/scripts/jquery.codecomb.fileupload.js");
            app.UseAutoAjax();
            app.UseIdentity();
            app.UseStaticFiles();
            app.UseSignalR();
            app.UseMvcWithDefaultRoute();

            await SampleData.InitDB(app.ApplicationServices);
            #region Timers
            this.NodeTimer = new Timer((t) =>
            {
                lock(this)
                {
                    using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
                    using (var db = serviceScope.ServiceProvider.GetService<ManoContext>())
                    {
                        var nodes = db.Nodes.ToList();
                        foreach (var node in nodes)
                        {
                            using (var client = new HttpClient())
                            {
                                try
                                {
                                    var task = client.GetAsync(node.Url + "/");
                                    task.Wait();
                                    var result = task.Result;
                                    if (result.StatusCode == System.Net.HttpStatusCode.OK)
                                        node.Status = NodeStatus.Online;
                                    else
                                        node.Status = NodeStatus.Offline;
                                }
                                catch
                                {
                                    node.Status = NodeStatus.Offline;
                                }
                                db.SaveChanges();
                            }
                        }
                    }
                }
            }, null, 0, 1000 * 5);

            this.SyncTimer = new Timer(async (t) =>
            {
                using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
                using (var db = serviceScope.ServiceProvider.GetService<ManoContext>())
                using (var client = new HttpClient())
                {
                    var time = DateTime.Now.AddDays(-1);
                    var projects = db.Projects
                        .Include(x => x.Node)
                        .Where(x => x.Type != CommunityType.None && time > x.LastPullTime)
                        .ToList();
                    foreach (var x in projects)
                    {
                        if (!x.NodeId.HasValue || x.Node.Status == NodeStatus.Offline)
                        {
                            var node = db.Nodes.FirstOrDefault(y => y.Status == NodeStatus.Online);
                            if (node == null)
                            {
                                Console.WriteLine($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}] No node is able to use for project {x.Title}.");
                                continue;
                            }
                            x.NodeId = node.Id;
                            db.SaveChanges();
                        }
                        var result = await client.PostAsync(x.Node.Url + "/Execute/" + x.Id, new FormUrlEncodedContent(new Dictionary<string, string>
                        {
                            { "source", x.ThirdPartyUrl}
                        }));
                        if (result.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            x.LastPullTime = DateTime.Now;
                            db.SaveChanges();
                        }
                        else
                        {
                            Console.WriteLine($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}] There is an error occurred with project {x.Title}.");
                        }
                    }
                }
            }, null, 1000 * 15, 1000 * 60 * 10);

            this.ProjectTimer = new Timer((t) =>
            {
                using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
                using (var db = serviceScope.ServiceProvider.GetService<ManoContext>())
                {
                    var time = DateTime.Now.AddDays(-1);
                    var users = db.Users
                        .Where(x => time > x.LastPullTime)
                        .ToList();
                    foreach (var x in users)
                    {
                        x.LastPullTime = DateTime.Now;
                        Controllers.AccountController.Sync(app.ApplicationServices, x);
                    }
                    db.SaveChanges();
                }
            }, null, 1000 * 5, 1000 * 60 * 60);
            #endregion
        }

        public static void Main(string[] args) => WebApplication.Run<Startup>(args);
    }
}