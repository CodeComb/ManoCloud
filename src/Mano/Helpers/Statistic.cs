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
        public static HtmlString Radar<TModel>(this IHtmlHelper<TModel> self, Project project)
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
                .Where(x => extensions.Contains(x.Id))
                .ToList();
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
            dataset1.data = project.Commits
                .SelectMany(x => x.Changes)
                .GroupBy(x => Path.GetExtension(x.Path))
                .Select(x => x.Sum(y => y.Additions + y.Deletions))
                .ToList();
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
                    .Where(y => Path.GetExtension(y.Path) == x.Id)
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
    }
}
