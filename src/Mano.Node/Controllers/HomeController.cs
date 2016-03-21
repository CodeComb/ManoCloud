using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using System.IO;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Mano.Parser;

namespace Mano.Node.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return Content("Mano cloud node is ok.");
        }

        [Route("/Execute/{id}")]
        public IActionResult Execute(string id, string source, [FromServices] IConfiguration Config)
        {
            Task.Factory.StartNew(() =>
            {
                var path = Path.Combine(Config["Pool"], "/" + id);
                var logs = GitLog.CloneAndGetLogs(source, path);
                var json = JsonConvert.SerializeObject(logs);
                var client = new HttpClient();
                long length = 0;
                foreach (var f in Directory.GetFiles(path, "*", SearchOption.AllDirectories))
                {
                    var fi = new FileInfo(f);
                    length += fi.Length;
                }
                client.PostAsync(Config["Api"] + "PushLogs", new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "id", id },
                    { "json", json },
                    { "size", (length / 1024.0 / 1024.0).ToString() }
                }));
            });
            return Content("ok");
        }
    }
}
