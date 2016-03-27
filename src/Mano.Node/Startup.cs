using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Mano.Parser;

namespace Mano.Node
{
    public class Startup
    {
        public static Queue<Models.PullTask> Queue = new Queue<Models.PullTask>();

        public bool IsPulling = false;

        private Timer PullTimer;

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddConfiguration();
        }

        public void Configure(IApplicationBuilder app, ILoggerFactory logger)
        {
            logger.MinimumLevel = LogLevel.Warning;
            logger.AddConsole();

            app.UseMvcWithDefaultRoute();

            var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
            var Config = serviceScope.ServiceProvider.GetRequiredService<IConfiguration>();

            PullTimer = new Timer((t) =>
            {
                if (IsPulling || Queue.Count == 0)
                    return;
                IsPulling = true;
                var task = Queue.Dequeue();
                Task.Factory.StartNew(async () =>
                {
                    try
                    {
                        Console.WriteLine($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}] Analyzing {task.Id}.");
                        var path = Config["Pool"] + task.Id;
                        if (!Directory.Exists(path))
                            Directory.CreateDirectory(path);
                        var logs = GitLog.CloneAndGetLogs(task.Source, path);
                        var json = JsonConvert.SerializeObject(logs);
                        var client = new HttpClient();
                        long length = 0;
                        foreach (var f in Directory.GetFiles(path, "*", SearchOption.AllDirectories))
                        {
                            var fi = new FileInfo(f);
                            length += fi.Length;
                        }
                        Console.WriteLine($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}] Pushing {task.Id}.");
                        using (var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(json)))
                        {
                            var content = new MultipartFormDataContent();
                            content.Add(new StringContent(task.Id), "id");
                            content.Add(new StringContent(Config["Key"]), "key");
                            content.Add(new StringContent((length / 1024.0 / 1024.0).ToString()), "size");
                            content.Add(new StreamContent(stream), "json", "logs.json");
                            var result = await client.PostAsync(Config["Api"] + "/PushLogs", content);
                            if (result.StatusCode != System.Net.HttpStatusCode.OK)
                                Console.WriteLine($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}] Failed: {task.Id}");
                            else
                                Console.WriteLine($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}] Pushed: {await result.Content.ReadAsStringAsync()}");
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
                    }
                    IsPulling = false;
                });
            }, null, 0, 5000);
        }

        public static void Main(string[] args) => WebApplication.Run<Startup>(args);
    }
}
