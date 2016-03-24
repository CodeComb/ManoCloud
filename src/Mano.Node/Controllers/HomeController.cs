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
            Task.Factory.StartNew(async () =>
            {
                var path = Config["Pool"] + id;
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                var logs = GitLog.CloneAndGetLogs(source, path);
                var json = JsonConvert.SerializeObject(logs);
                var client = new HttpClient();
                long length = 0;
                foreach (var f in Directory.GetFiles(path, "*", SearchOption.AllDirectories))
                {
                    var fi = new FileInfo(f);
                    length += fi.Length;
                }
                using (var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(json)))
                { 
                    var content = new MultipartFormDataContent("Upload----" + DateTime.Now.ToString("MM-dd-yyyy HH:mm:ss"));
                    content.Add(new StringContent(id), "id");
                    content.Add(new StringContent(Config["Key"]), "key");
                    content.Add(new StringContent((length / 1024.0 / 1024.0).ToString()),"size");
                    content.Add(new StreamContent(stream), "json", "logs.json");
                    var result = await client.PostAsync(Config["Api"] + "/PushLogs", content);
                    if (result.StatusCode != System.Net.HttpStatusCode.OK)
                        Console.WriteLine($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}] {id} Error.");
                }
            });
            return Content("ok");
        }
    }
}
