using System;
using Microsoft.AspNetCore.Mvc;
using Pomelo.Net.Smtp;
using Mano.Models;

namespace Mano.Controllers
{
    public class BaseController : BaseController<ManoContext, User, long>
    {
        [Inject]
        public IEmailSender Mail { get; set; }

        [Inject]
        public AesCrypto Aes { get; set; }
    }
}
