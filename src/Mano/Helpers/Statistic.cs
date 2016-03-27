using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Mano.Parser.Models;
using Mano.Models;

namespace Mano.Helpers
{
    public static class Statistic
    {
        public static List<StatisticsRaw> ProjectRaw(ManoContext DB, Guid pid, List<Commit> commits)
        {
            var project = DB.Projects.Single(x => x.Id == pid);
            var emails = DB.Emails.Where(x => x.UserId == project.UserId).Select(x => x.EmailAddress).ToList();
            if (commits
                .Where(x =>emails.Contains(x.Email))
                .Count() == 0)
                return new List<StatisticsRaw>();

            var ret = commits
                .GroupBy(x => DB.Extensions.Where(y => y.Id == x.Extension && (y.Type == TechnologyType.编程语言 || y.Type == TechnologyType.序列化格式)).SingleOrDefault() != null
                ? new { Type = DB.Extensions.Where(y => y.Id == x.Extension && (y.Type == TechnologyType.编程语言 || y.Type == TechnologyType.序列化格式)).SingleOrDefault().Type, Technology = DB.Extensions.Where(y => y.Id == x.Extension && (y.Type == TechnologyType.编程语言 || y.Type == TechnologyType.序列化格式)).SingleOrDefault().Technology }
                : new { Type = TechnologyType.其他, Technology = "其他" })
                .Select(x => new StatisticsRaw
                {
                    Technology = x.Key != null ? x.Key.Technology : "其他",
                    Mine = x.Where(y => emails.Contains(y.Email)).Count() > 0 ? x.Where(y => emails.Contains(y.Email)).Sum(y => y.Additions + y.Deletions) : 0,
                    Total = x.Count() > 0 ? x.Sum(y => y.Additions + y.Deletions) : 0,
                    Begin = x.Where(y => emails.Contains(y.Email)).Count() > 0 ? x.Where(y => emails.Contains(y.Email)).Min(y => y.Time) : DateTime.Now,
                    End = x.Where(y => emails.Contains(y.Email)).Count() > 0 ? x.Where(y => emails.Contains(y.Email)).Max(y => y.Time) : DateTime.Now,
                    Type = x.Key != null ? x.Key.Type : TechnologyType.其他
                })
                .ToList();

            return ret;
        }

        public static List<StatisticsRaw> UserRaw(ManoContext DB, long uid)
        {
            var projects = DB.Projects.Where(x => x.UserId == uid).Select(x => x.Statistics).ToList();
            return projects
                .SelectMany(x => JsonConvert.DeserializeObject<List<StatisticsRaw>>(x))
                .GroupBy(x => new { Technology = x.Technology, Type = x.Type })
                .Select(x => new StatisticsRaw
                {
                    Technology = x.Key.Technology,
                    Mine = x.Sum(y => y.Mine),
                    Total = x.Sum(y => y.Total),
                    Begin = x.Min(y => y.Begin),
                    End = x.Max(y => y.End),
                    Type = x.Key.Type
                })
                .ToList();
        }

        public static HtmlString Radar (this IHtmlHelper self, string json, bool mine = false)
        {
            var charts = JsonConvert.DeserializeObject<List<StatisticsRaw>>(json);
            var radar = new RadarChart();
            radar.labels = charts.Select(x => x.Technology).ToList();
            if (!mine)
            {
                radar.datasets.Add(new RadarChartDataSet
                {
                    fillColor = "rgba(220,220,220,0.5)",
                    strokeColor = "rgba(220,220,220,1)",
                    pointColor = "rgba(220,220,220,1)",
                    pointStrokeColor = "#fff",
                    data = charts.Select(x => x.Total).ToList()
                });
            }
            radar.datasets.Add(new RadarChartDataSet
            {
                fillColor = "rgba(151,187,205,0.5)",
                strokeColor = "rgba(151,187,205,1)",
                pointColor = "rgba(151,187,205,1)",
                pointStrokeColor = "#fff",
                data = charts.Select(x => x.Mine).ToList()
            });
            return new HtmlString(JsonConvert.SerializeObject(radar));
        }
    }
}
