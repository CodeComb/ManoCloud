using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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
            var Config = HttpContext.RequestServices.GetRequiredService<IConfiguration>();
            if (Request.Cookies["ASPNET_TEMPLATE"] != "Default")
            {
                Response.Cookies.Append("ASPNET_TEMPLATE", "Default");
                Response.Redirect("//" + Config["Host"]);
            }
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
