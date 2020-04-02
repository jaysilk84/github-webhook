using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace JaySilk.Webhook.Common.Mvc.Extensions
{

    public class GitHubSignatureMiddleware : VerifySignatureMiddleware
    {
        private const string DEFAULT_HEADER_NAME = "X-Hub-Signature";

        public GitHubSignatureMiddleware(RequestDelegate next, IOptions<SignatureOptions> config)
            : base(next, DEFAULT_HEADER_NAME, config.Value.Secret, x => x.Substring(5)) { }

    }
}
