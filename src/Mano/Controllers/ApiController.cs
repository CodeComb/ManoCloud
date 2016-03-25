using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using Microsoft.Data.Entity;
using Newtonsoft.Json;
using Mano.Models;

namespace Mano.Controllers
{
    public class ApiController : BaseController
    {
        [HttpPost]
        public IActionResult PushLogs(Guid id, IFormFile json, double size, string key)
        {
            if (DB.Nodes.SingleOrDefault(x => x.Key == key) == null)
                return Content("no");
            var project = DB.Projects.Single(x => x.Id == id);
            project.Size = size;
            var Json = Encoding.UTF8.GetString(json.ReadAllBytes());
            var commits = JsonConvert.DeserializeObject<List<Commit>>(Json);
            foreach (var x in commits)
            {
                if (DB.Commits.SingleOrDefault(y => y.Id == x.Id) == null)
                {
                    x.ProjectId = id;
                    DB.Commits.Add(x);
                }
            }
            DB.SaveChanges();
            lock(this)
            {
                var emails = DB.Emails
                    .Where(x => x.Verified && x.UserId == project.UserId)
                    .Select(x => x.EmailAddress)
                    .ToList();
                var tech = DB.Extensions
                    .Where(x => x.Type == TechnologyType.编程语言 || x.Type == TechnologyType.序列化格式)
                    .ToDictionary(x => x.Id);
                var statistics = DB.Commits
                    .Include(x => x.Changes)
                    .ThenInclude(x => x.Commit)
                    .Where(x => x.ProjectId == id)
                    .SelectMany(x => x.Changes)
                    .Where(x => tech.ContainsKey(Path.GetExtension(x.Path)))
                    .GroupBy(x => tech[Path.GetExtension(x.Path)])
                    .Select(x => new { Key = x.Key, Count = x.Sum(y => y.Additions + y.Deletions), Begin = x.Min(y => y.Commit.Time) })
                    .ToList();
                foreach (var x in statistics)
                {
                    if (DB.Skills.Where(y => y.UserId == project.UserId && y.UpdateFromGit && y.Title == x.Key.Technology).Count() > 0)
                    {
                        var skill = DB.Skills.Where(y => y.UserId == project.UserId && y.UpdateFromGit && y.Title == x.Key.Technology).Single();
                        skill.Count = x.Count;
                        skill.Begin = x.Begin;
                    }
                    else
                    {
                        DB.Skills.Add(new Skill
                        {
                            Begin = x.Begin,
                            Title = x.Key.Technology,
                            Count = x.Count,
                            Unit = "行",
                            Type = x.Key.Type,
                            UserId = project.UserId,
                            UpdateFromGit = true
                        });
                    }
                }
                DB.SaveChanges();
            }
            return Content("ok");
        }
    }
}
