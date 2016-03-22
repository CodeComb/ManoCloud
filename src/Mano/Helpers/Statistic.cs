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
                .SelectMany(x => x.Changes)
                .Select(x => Path.GetExtension(x.Path))
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
                    .SelectMany(y => y.Changes)
                    .Where(y => x.Type == dic[Path.GetExtension(y.Path)].Value)
                    .Sum(y => y.Additions + y.Deletions);
                dataset1.data.Add(cnt);
            }
            if (radar.labels.Contains("其他"))
            {
                var cnt = project.Commits
                    .SelectMany(y => y.Changes)
                    .Where(y => !map.Select(x => x.Id).Contains(Path.GetExtension(y.Path)))
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
                    .Where(y => emails.Contains(y.Email))
                    .SelectMany(y => y.Changes)
                    .Where(y => x.Type == dic[Path.GetExtension(y.Path)].Value)
                    .Sum(y => y.Additions + y.Deletions);
                dataset2.data.Add(cnt);
            }
            if (radar.labels.Contains("其他"))
            {
                var cnt = project.Commits
                    .Where(y => emails.Contains(y.Email))
                    .SelectMany(y => y.Changes)
                    .Where(y => !map.Select(x => x.Id).Contains(Path.GetExtension(y.Path)))
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
                .SelectMany(x => x.Changes)
                .Select(x => Path.GetExtension(x.Path))
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
                var cnt = projects
                    .SelectMany(y => y.Commits)
                    .SelectMany(y => y.Changes)
                    .Where(y => x.Type == dic[Path.GetExtension(y.Path)].Value)
                    .Sum(y => y.Additions + y.Deletions);
                dataset1.data.Add(cnt);
            }
            if (radar.labels.Contains("其他"))
            {
                var cnt = projects
                    .SelectMany(y => y.Commits)
                    .SelectMany(y => y.Changes)
                    .Where(y => !map.Select(x => x.Id).Contains(Path.GetExtension(y.Path)))
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
                    .Where(y => emails.Contains(y.Email))
                    .SelectMany(y => y.Changes)
                    .Where(y => x.Type == dic[Path.GetExtension(y.Path)].Value)
                    .Sum(y => y.Additions + y.Deletions);
                dataset2.data.Add(cnt);
            }
            if (radar.labels.Contains("其他"))
            {
                var cnt = projects
                    .SelectMany(y => y.Commits)
                    .Where(y => emails.Contains(y.Email))
                    .SelectMany(y => y.Changes)
                    .Where(y => !map.Select(x => x.Id).Contains(Path.GetExtension(y.Path)))
                    .Sum(y => y.Additions + y.Deletions);
                dataset2.data.Add(cnt);
            }
            radar.datasets.Add(dataset1);
            radar.datasets.Add(dataset2);
            return new HtmlString(JsonConvert.SerializeObject(radar));
        }
    }
}
