using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace JaySilk.Webhook.Common.Mvc
{

    //TODO: Inherit VerifySignatureMiddleware and clean all this up. Move to Extensions folder, change namespace, etc
    public class GitHubSignatureMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IOptions<SignatureOptions> _config;
        private const string DEFAULT_HEADER_NAME = "X-Hub-Signature";

        public GitHubSignatureMiddleware(RequestDelegate next, IOptions<SignatureOptions> config)
        {
            _next = next;
            _config = config;
        }

        public async Task Invoke(HttpContext context)
        {
            var result = await context.VerifySignatureFromHeaderAsync(_config.Value.SignatureHeaderName ?? DEFAULT_HEADER_NAME, _config.Value.Secret);
            if (!result.IsValid)
                await Fail(context, result.Message);
            else
                await _next(context);
        }

        private async Task Fail(HttpContext context, string message)
        {
            if (!context.Response.HasStarted) {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                await context.Response.WriteAsync(message);
            } else {
                throw new InvalidOperationException("Response has started");
            }
        }
    }
}
