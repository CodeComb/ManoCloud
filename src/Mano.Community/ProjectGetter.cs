using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace Mano.Community
{
    public static class ProjectGetter
    {
        public static async Task<List<Project>> FromGitHub(string Username)
        {
            var url = $"https://github.com/{Username}?tab=repositories";
            var result = await Http.Get(url);
            var regex = new Regex("(?<=<a href=\").*?(?=\" itemprop=\"name codeRepository\">)");
            var ret = new List<string>();
            foreach (Match x in regex.Matches(result))
                ret.Add(x.Value);
            return ret
                .Select(x => new Project
                {
                    Git = $"https://github.com{x}.git",
                    Title = x.Split('/')[2]
                })
                .ToList();
        }

        public static async Task<List<Project>> FromCodePlex(string Username)
        {
            var url = $"http://www.codeplex.com/site/users/view/{Username}";
            var result = await Http.Get(url);
            var regex = new Regex("(?<=<a href=\"http://).*?(?=.codeplex.com/\" title=\".*?\">.*?<br /></a>)");
            var ret = new List<string>();
            foreach (Match x in regex.Matches(result))
                ret.Add(x.Value);
            return ret
                .Select(x => new Project
                {
                    Git = $"https://git01.codeplex.com/{x}",
                    Title = x
                })
                .ToList();
        }

        public static async Task<List<Project>> FromGitOSC(string Username)
        {
            var url = $"https://git.oschina.net/{Username}/projects";
            var result = await Http.Get(url);
            var regex = new Regex("(?<=<span class='pro-name'>[\\r\\n]<span>(.*?)<a href=\")[a-zA-Z0-9-_./]{0,}(?=\" class=\"repository\" style=\"padding-bottom: 0px\")");
            var ret = new List<string>();
            foreach (Match x in regex.Matches(result))
                ret.Add(x.Value);
            return ret
                .Select(x => new Project
                {
                    Git = $"https://git.oschina.net{x}.git",
                    Title = x.Split('/')[2]
                })
                .ToList();
        }

        public static async Task<List<Project>> FromCsdnCode(string Username)
        {
            var url = $"https://code.csdn.net/{Username}";
            var result = await Http.Get(url);
            var regex = new Regex("(?<=<a href=\").*?(?=\" class=\"project_title_limit cutout\">)");
            var ret = new List<string>();
            foreach (Match x in regex.Matches(result))
                ret.Add(x.Value);
            return ret
                .Select(x => new Project
                {
                    Git = $"https://code.csdn.net{x}.git",
                    Title = x.Split('/')[2]
                })
                .ToList();
        }

        public static async Task<List<Project>> FromCodingNet(string Username)
        {
            var url = $"https://coding.net/api/user/{Username}/public_projects?type=joined&pagesize=60000";
             var result = await Http.Get(url);
            var obj = JsonConvert.DeserializeObject<dynamic>(result);
            var ret = new List<Project>();
            foreach (var x in obj.data.list)
            {
                ret.Add(new Project
                {
                    Title = x.name,
                    Git = x.https_url
                });
            }
            return ret;
        }
    }
}
