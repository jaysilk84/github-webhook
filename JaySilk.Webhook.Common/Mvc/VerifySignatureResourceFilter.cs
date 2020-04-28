using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Net;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using JaySilk.Webhook.Common.Security;

namespace JaySilk.Webhook.Common.Mvc
{

    /// <summary>
    /// Filter to read a signature header, parse the body, and verify the signature with the shared
    /// secret. Implemented as a resource filter because it requires access to the http context.
    /// </summary>
    public abstract class VerifySignatureResourceFilter : VerifySignature<ResourceExecutingContext>, IAsyncResourceFilter
    {
        public VerifySignatureResourceFilter(string signatureHeaderName, string secret, Func<string, string> headerValueTransformer)
            : base(signatureHeaderName, secret, headerValueTransformer)
        {

        }

        /// <summary>
        /// Runs prior to the action and validates the signature
        /// </summary>
        /// <param name="context"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
        {
            if (await Verify(context, context.HttpContext))
                await next();
        }

        protected override async Task OnSuccess(ResourceExecutingContext context) => await Task.CompletedTask; // noop

        /// <summary>
        /// Generate a 403 failure with a message
        /// </summary>
        /// <param name="context"></param>
        /// <param name="message"></param>
        protected override Task FailAsync(ResourceExecutingContext context, string message)
        {
            context.Result = new ContentResult()
            {
                StatusCode = (int)HttpStatusCode.Unauthorized,
                Content = message
            };

            return Task.CompletedTask;
        }
    }
}
