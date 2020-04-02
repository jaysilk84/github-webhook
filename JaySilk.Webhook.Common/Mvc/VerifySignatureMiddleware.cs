using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace JaySilk.Webhook.Common.Mvc
{
    public abstract class VerifySignatureMiddleware : VerifySignature<HttpContext>
    {
        private readonly RequestDelegate _next;
      
        public VerifySignatureMiddleware(RequestDelegate next, string signatureHeaderName, string secret, Func<string, string> headerValueTransformer) 
            : base(signatureHeaderName, secret, headerValueTransformer)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context) => await Verify(context, context);

        protected override async Task OnSuccess(HttpContext context) => await _next(context);
        
        protected override async Task FailAsync(HttpContext context, string message)
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
