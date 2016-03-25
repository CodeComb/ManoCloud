using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Mano.Models;

namespace Mano.Helpers
{
    public static class Statistic
    {
        public static HtmlString ProjectRadar<TModel>(this IHtmlHelper<TModel> self, Project project)
            where TModel : User
        {
            var emails = self.ViewData.Model.Emails
                .Select(x => x.EmailAddress)
                .ToList();
            var radar = new RadarChart();
            var extensions = project.Commits
                .Select(x => x.Extension)
                .Distinct()
                .ToList();
            var DB = self.ViewContext.HttpContext.RequestServices.GetRequiredService<ManoContext>();
            var map = DB.Extensions
                .Where(x => extensions.Contains(x.Id) && (x.Type == TechnologyType.编程语言 || x.Type == TechnologyType.序列化格式))
                .ToList();
            var dic = map
                .Select(x => new { Key = x.Id, Value = x.Type })
                .ToDictionary(x => x.Key);
            radar.labels = map
                .Select(x => x.Technology)
                .ToList();
            foreach (var x in extensions)
            {
                if (!map.Any(y => y.Id == x))
                {
                    radar.labels.Add("其他");
                    break;
                }
            }
            var dataset1 = new RadarChartDataSet
            {
                fillColor = "rgba(220,220,220,0.5)",
                strokeColor = "rgba(220,220,220,1)",
                pointColor = "rgba(220,220,220,1)",
                pointStrokeColor = "#fff"
            };
            foreach (var x in map)
            {
                var cnt = project.Commits
                    .Where(y => dic.ContainsKey(y.Extension) && x.Type == dic[y.Extension]?.Value)
                    .Sum(y => y.Additions + y.Deletions);
                dataset1.data.Add(cnt);
            }
            if (radar.labels.Contains("其他"))
            {
                var cnt = project.Commits
                    .Where(y => !dic.ContainsKey(y.Extension))
                    .Sum(y => y.Additions + y.Deletions);
                dataset1.data.Add(cnt);
            }
            var dataset2 = new RadarChartDataSet
            {
                fillColor = "rgba(151,187,205,0.5)",
                strokeColor = "rgba(151,187,205,1)",
                pointColor = "rgba(151,187,205,1)",
                pointStrokeColor = "#fff"
            };
            foreach (var x in map)
            {
                var cnt = project.Commits
                    .Where(y => emails.Contains(y.Email) && dic.ContainsKey(y.Extension) && x.Type == dic[y.Extension]?.Value)
                    .Sum(y => y.Additions + y.Deletions);
                dataset2.data.Add(cnt);
            }
            if (radar.labels.Contains("其他"))
            {
                var cnt = project.Commits
                    .Where(y => emails.Contains(y.Email))
                    .Where(y => !dic.ContainsKey(y.Extension))
                    .Sum(y => y.Additions + y.Deletions);
                dataset2.data.Add(cnt);
            }
            radar.datasets.Add(dataset1);
            radar.datasets.Add(dataset2);
            return new HtmlString(JsonConvert.SerializeObject(radar));
        }
        public static HtmlString UserRadar<TModel>(this IHtmlHelper<TModel> self, IEnumerable<Project> projects)
            where TModel : User
        {
            var emails = self.ViewData.Model.Emails
                .Select(x => x.EmailAddress)
                .ToList();
            var radar = new RadarChart();
            var extensions = projects
                .SelectMany(x => x.Commits)
                .Select(x => x.Extension)
                .Distinct()
                .ToList();
            var DB = self.ViewContext.HttpContext.RequestServices.GetRequiredService<ManoContext>();
            var map = DB.Extensions
                .Where(x => extensions.Contains(x.Id) && (x.Type == TechnologyType.编程语言 || x.Type == TechnologyType.序列化格式))
                .ToList();
            var dic = map
                .Select(x => new { Key = x.Id, Value = x.Technology })
                .ToDictionary(x => x.Key);
            radar.labels = map
                .Select(x => x.Technology)
                .ToList();
            foreach (var x in extensions)
            {
                if (!map.Any(y => y.Id == x))
                {
                    radar.labels.Add("其他");
                    break;
                }
            }
            var dataset1 = new RadarChartDataSet
            {
                fillColor = "rgba(220,220,220,0.5)",
                strokeColor = "rgba(220,220,220,1)",
                pointColor = "rgba(220,220,220,1)",
                pointStrokeColor = "#fff"
            };
            foreach (var x in map)
            {
                var cnt = projects
                    .SelectMany(y => y.Commits)
                    .Where(y => dic.ContainsKey(y.Extension) && x.Technology == dic[y.Extension]?.Value)
                    .Sum(y => y.Additions + y.Deletions);
                dataset1.data.Add(cnt);
            }
            if (radar.labels.Contains("其他"))
            {
                var cnt = projects
                    .SelectMany(y => y.Commits)
                    .Where(y => !dic.ContainsKey(y.Extension))
                    .OrderByDescending(y => y.Additions + y.Deletions)
                    .Sum(y => y.Additions + y.Deletions);
                dataset1.data.Add(cnt);
            }
            var dataset2 = new RadarChartDataSet
            {
                fillColor = "rgba(151,187,205,0.5)",
                strokeColor = "rgba(151,187,205,1)",
                pointColor = "rgba(151,187,205,1)",
                pointStrokeColor = "#fff"
            };
            foreach (var x in map)
            {
                var cnt = projects
                    .SelectMany(y => y.Commits)
                    .Where(y => emails.Contains(y.Email) && dic.ContainsKey(y.Extension) && x.Technology == dic[y.Extension].Value)
                    .Sum(y => y.Additions + y.Deletions);
                dataset2.data.Add(cnt);
            }
            if (radar.labels.Contains("其他"))
            {
                var cnt = projects
                    .SelectMany(y => y.Commits)
                    .Where(y => emails.Contains(y.Email))
                    .Where(y => !dic.ContainsKey(y.Extension))
                    .Sum(y => y.Additions + y.Deletions);
                dataset2.data.Add(cnt);
            }
            radar.datasets.Add(dataset1);
            radar.datasets.Add(dataset2);
            return new HtmlString(JsonConvert.SerializeObject(radar));
        }
        public static List<SkillStatistics> UserSkill<TModel>(this IHtmlHelper<TModel> self, IEnumerable<Project> projects)
            where TModel : User
        {
            var emails = self.ViewData.Model.Emails
                .Select(x => x.EmailAddress)
                .ToList();
            var changes = projects
                .SelectMany(x => x.Commits)
                .Where(x => emails.Contains(x.Email))
                .ToList();
            var DB = self.ViewContext.HttpContext.RequestServices.GetRequiredService<ManoContext>();
            var dic = DB.Extensions
                .Where(x => changes.Select(y => Path.GetExtension(y.Path)).Contains(x.Id) && (x.Type == TechnologyType.编程语言 || x.Type == TechnologyType.序列化格式))
                .Select(x => new { Key = x.Id, Value = x.Type, Technology = x.Technology })
                .ToDictionary(x => x.Key);
            var ret = changes
                .Where(x => dic.ContainsKey(x.Extension))
                .GroupBy(x =>dic[x.Extension])
                .Select(x => new SkillStatistics
                {
                    Begin = x.Min(y => y.Time),
                    Skill = x.Key.Technology.ToString(),
                    Count = x.Sum(y => y.Additions + y.Deletions)
                })
                .ToList();
            var others = changes
                .Where(x => !dic.ContainsKey(x.Extension))
                .ToList();
            if (others.Count > 0)
            {
                ret.Add(new SkillStatistics
                {
                    Begin = others.Min(x => x.Time),
                    Count = others.Sum(x => x.Additions + x.Deletions),
                    Skill = "其他"
                });
            }
            ret = ret.OrderByDescending(x => x.Count).ToList();
            return ret;
        }
        public static ProjectStatistics ProjectStatistics<TModel>(this IHtmlHelper<TModel> self, Project project)
            where TModel : User
        {
            var emails = self.ViewData.Model.Emails
                .Select(x => x.EmailAddress)
                .ToList();
            var ret = new ProjectStatistics();
            if (project.Commits.Count > 0)
            {
                ret.Begin = project.Commits.Min(x => x.Time);
                ret.End = project.Commits.Max(x => x.Time);
            }
            else
            {
                ret.Begin = DateTime.Now;
                ret.End = DateTime.Now;
            }
            ret.Count = project.Commits.Sum(x =>x.Additions + x.Deletions);
            ret.Contributed = project.Commits.Where(x => emails.Contains(x.Email)).Sum(x => x.Additions + x.Deletions);
            ret.Contributors = project.Commits.Select(x => x.Email).Distinct().Count();
            return ret;
        }
    }
}
