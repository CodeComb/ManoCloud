using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Pomelo.AspNetCore.TimedJob;
using Mano.Models;

namespace Mano.Jobs
{
    public class ManoBeatJob : Job
    {
        [Invoke(Interval = 5000, SkipWhileExecuting = true)]
        public void HeartBeat(ManoContext DB)
        {
            var nodes = DB.Nodes.ToList();
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
                    DB.SaveChanges();
                }
            }
        }

        [Invoke(Interval = 1000 * 60 * 10, SkipWhileExecuting = true)]
        public async void SyncRepositories(ManoContext DB)
        {
            using (var client = new HttpClient())
            {
                var time = DateTime.Now.AddDays(-1);
                var projects = DB.Projects
                    .Include(x => x.Node)
                    .Where(x => x.Type != CommunityType.None && time > x.LastPullTime)
                    .ToList();
                foreach (var x in projects)
                {
                    if (!x.NodeId.HasValue || x.Node.Status == NodeStatus.Offline)
                    {
                        var node = DB.Nodes.Where(y => y.Status == NodeStatus.Online).OrderByDescending(a => a.MaxSize - (DB.Projects.Where(y => y.NodeId == a.Id).Count() > 0 ? DB.Projects.Where(y => y.NodeId == a.Id).Sum(z => z.Size) : 0)).FirstOrDefault();
                        if (node == null)
                        {
                            Console.WriteLine($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}] No node is able to use for project {x.Title}.");
                            continue;
                        }
                        x.NodeId = node.Id;
                        DB.SaveChanges();
                    }
                    var result = await client.PostAsync(x.Node.Url + "/Execute/" + x.Id, new FormUrlEncodedContent(new Dictionary<string, string>
                        {
                            { "source", x.ThirdPartyUrl}
                        }));
                    if (result.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        x.LastPullTime = DateTime.Now;
                        DB.SaveChanges();
                    }
                    else
                    {
                        Console.WriteLine($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}] There is an error occurred with project {x.Title}.");
                    }
                }
            }
        }

        [Invoke(Interval = 1000 * 60 * 60, SkipWhileExecuting = true)]
        public void ProjectPulling(ManoContext DB, IServiceProvider services)
        {
            var time = DateTime.Now.AddDays(-1);
            var users = DB.Users
                .Where(x => time > x.LastPullTime)
                .ToList();
            foreach (var x in users)
            {
                x.LastPullTime = DateTime.Now;
                Controllers.AccountController.Sync(services, x);
            }
            DB.SaveChanges();
        }
    }
}
