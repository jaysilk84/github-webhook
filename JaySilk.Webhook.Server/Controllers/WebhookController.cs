using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using JaySilk.Webhook.Common.Filter;

namespace JaySilk.Webhook.Server.Controllers
{

    [ApiController]
    [Route("[controller]")]

    public class WebhookController : ControllerBase
    {

        [HttpPost]
        [VerifySignatureResourceFilter]
        public void Post()
        {

        }

    }


}
