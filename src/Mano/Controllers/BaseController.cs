using Microsoft.AspNet.Mvc;
using CodeComb.Net.EmailSender;
using CodeComb.Security.Aes;
using Mano.Models;

namespace Mano.Controllers
{
    public class BaseController : BaseController<ManoContext, User, long>
    {
        [FromServices]
        public IEmailSender Mail { get; set; }

        [FromServices]
        public AesCrypto Aes { get; set; }
    }
}
