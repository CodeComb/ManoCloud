using Microsoft.AspNet.Mvc;

namespace Mano.Node.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return Content("Mano cloud node is ok.");
        }
    }
}
