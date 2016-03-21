using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Newtonsoft.Json;
using Mano.Models;
using Mano.Parser.Models;

namespace Mano.Controllers
{
    public class ApiController : BaseController
    {
        [HttpPost]
        public IActionResult PushLogs(Guid id, string json, double size, string key)
        {
            if (DB.Nodes.SingleOrDefault(x => x.Key == key) == null)
                return Content("no");
            var project = DB.Projects.Single(x => x.Id == id);
            project.Size = size;
            var commits = JsonConvert.DeserializeObject<List<Commit>>(json);
            foreach (var x in commits)
                if (DB.Commits.SingleOrDefault(y => y.Id == x.Id) == null)
                    DB.Commits.Add(x);
            DB.SaveChanges();
            return Content("ok");
        }
    }
}
