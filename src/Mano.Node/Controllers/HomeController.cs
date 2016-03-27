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
            Startup.Queue.Enqueue(new Models.PullTask { Id = id, Source = source });
            return Content("ok");
        }
    }
}
