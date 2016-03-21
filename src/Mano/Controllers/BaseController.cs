using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Mano.Models;

namespace Mano.Controllers
{
    public class BaseController : BaseController<ManoContext, User, long>
    {
    }
}
