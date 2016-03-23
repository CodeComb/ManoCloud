using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Mano.Controllers
{
    public class CoderController : BaseController
    {
        public override void Prepare()
        {
            base.Prepare();
            Cookies["ASPNET_TEMPLATE"] = "Default";
            var Config = Resolver.GetRequiredService<IConfiguration>();
            if (Request.Host.ToString() != Config["Host"])
                Response.Redirect("//" + Config["Host"]);
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
