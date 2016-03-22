using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.Data.Entity;
using Microsoft.Extensions.Configuration;

namespace Mano.Controllers
{
    public class HomeController : BaseController
    {
        public IActionResult Index([FromServices] IConfiguration Config)
        {
            var host = Request.Host.ToString();
            if (host == Config["Host"])
            {
                Cookies["ASPNET_TEMPLATE"] = "Default";
                return View();
            }
            var domain = DB.Domains
                .Include(x => x.User)
                .ThenInclude(x => x.Domains)
                .SingleOrDefault(x => x.DomainName == host);
            if (domain == null)
                return Redirect(Config["Home"]);
            if (!domain.Default)
                return Redirect("//" + domain.User.Domains.First(x => x.Default).DomainName);
            var user = DB.Users
                .Include(x => x.Skills)
                .Include(x => x.Experiences)
                .Include(x => x.Certifications)
                .Include(x => x.Educations)
                .Include(x => x.Projects)
                .ThenInclude(x => x.Commits)
                .ThenInclude(x => x.Changes)
                .Single(x => x.Id == domain.UserId);

            Cookies["ASPNET_TEMPLATE"] = user.Template ?? "Metro";

            ViewBag.TotalP = user.Skills
                .Where(x => x.Type == Models.TechnologyType.编程语言)
                .Max(x => x.TotalDays);
            ViewBag.TotalF = user.Skills
                .Where(x => x.Type != Models.TechnologyType.编程语言)
                .Max(x => x.TotalDays);

            return View(user);
        }
    }
}
