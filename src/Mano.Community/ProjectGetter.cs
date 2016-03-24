using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Mano.Community
{
    public static class ProjectGetter
    {
        public static async Task<List<string>> FromGitHub(string Username)
        {
            var url = $"https://github.com/{Username}?tab=repositories";
            var result = await Http.Get(url);
            var regex = new Regex("(?<=a href=\").*?(?=\" itemprop=\"name codeRepository\">)");
            var ret = new List<string>();
            foreach (Match x in regex.Matches(result))
                ret.Add(x.Value);
            return ret
                .Select(x => $"https://github.com{x}.git")
                .ToList();
        }
    }
}
