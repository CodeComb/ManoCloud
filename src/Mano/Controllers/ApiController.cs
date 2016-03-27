using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Data.Entity;
using Newtonsoft.Json;
using Mano.Models;
using Mano.Parser.Models;

namespace Mano.Controllers
{
    public class ApiController : BaseController
    {
        [HttpPost]
        public IActionResult PushLogs(Guid id, IFormFile json, double size, string key)
        {
            if (DB.Nodes.SingleOrDefault(x => x.Key == key) == null)
                return Content("no");
            Console.WriteLine($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}] Receiving: {id}");
            var project = DB.Projects
                .Include(x => x.User)
                .Single(x => x.Id == id);
            project.Size = size;
            var Json = Encoding.UTF8.GetString(json.ReadAllBytes());
            var commits = JsonConvert.DeserializeObject<List<Commit>>(Json);

            var ps = Helpers.Statistic.ProjectRaw(DB, project.Id, commits);
            project.Statistics = JsonConvert.SerializeObject(ps);
            if (ps.All(x => x.Mine == 0))
            {
                project.IsContributed = false;
                project.Begin = DateTime.Now;
            }
            else
            {
                project.IsContributed = true;
                project.Begin = ps.Min(x => x.Begin);
                project.End = ps.Max(x => x.End);
            }

            var us = Helpers.Statistic.UserRaw(DB, project.UserId);
            project.User.Statistics = JsonConvert.SerializeObject(us);

            foreach (var x in us)
            {
                if (DB.Skills.Where(y => y.UserId == project.UserId && y.UpdateFromGit && y.Title == x.Technology).Count() > 0)
                {
                    var skill = DB.Skills.Where(y => y.UserId == project.UserId && y.UpdateFromGit && y.Title == x.Technology).Single();
                    skill.Count = x.Mine;
                    skill.Begin = x.Begin;
                }
                else
                {
                    DB.Skills.Add(new Skill
                    {
                        Begin = x.Begin,
                        Title = x.Technology,
                        Count = x.Mine,
                        Unit = "行",
                        Type = x.Type,
                        UserId = project.UserId,
                        UpdateFromGit = true
                    });
                }
            }

            DB.SaveChanges();

            GC.Collect();
            return Content("ok");
        }
    }
}
