using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace JaySilk.Webhook.Common.Mvc
{
    // TODO: Make sure the public/private APIs are good for these abstract bases

    public abstract class VerifySignatureMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly SignatureOptions _config;

        public VerifySignatureMiddleware(RequestDelegate next, SignatureOptions config)
        {
            _next = next;
            _config = config;
        }

        public async Task Invoke(HttpContext context)
        {
            // TODO: Should we worry about HeaderValueTransformer being null? It's technically exposed to users (see SignatureOptions comment)
            var result = await context.VerifySignatureFromHeaderAsync(_config.SignatureHeaderName, _config.Secret, _config.HeaderValueTransformer);
            if (!result.IsValid)
                await Fail(context, result.Message);
            else
                await _next(context);
        }

        protected async Task Fail(HttpContext context, string message)
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
