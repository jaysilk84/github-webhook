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
using JaySilk.Webhook.Common.Math;

namespace JaySilk.Webhook.Common.Mvc
{

    /// <summary>
    /// Filter to read a signature header, parse the body, and verify the signature with the shared
    /// secret. Implemented as a resource filter because it requires access to the http context.
    /// </summary>
    public class VerifySignatureResourceFilter : Attribute, IAsyncResourceFilter
    {
        // TODO: Clean all the properties up, should be fields, make class abstract, ponder the SignatureOptions question
        protected const int BUFFER_SIZE = 1024 * 45; // 45kb
        protected string SignatureHeaderName { get; }
        protected string Secret { get; }

        protected Func<string, string> HeaderValueTransformer;
        protected static Func<string, string> NoOpValueTransformer = (s) => s;

        public VerifySignatureResourceFilter(string signatureHeaderName, string secret, Func<string, string> headerValueTransformer)
        {
            SignatureHeaderName = signatureHeaderName;
            Secret = secret;
            HeaderValueTransformer = headerValueTransformer;
        }

        public VerifySignatureResourceFilter(string signatureHeaderName, string secret) : this(signatureHeaderName, secret, NoOpValueTransformer) { }

        /// <summary>
        /// Runs prior to the action and validates the signature
        /// </summary>
        /// <param name="context"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
        {
            var result = await context.HttpContext.VerifySignatureFromHeaderAsync(SignatureHeaderName, Secret, HeaderValueTransformer);
            if (result.IsValid)
                await next();
            else
                Fail(context, result.Message);
        }

        /// <summary>
        /// Generate a 403 failure with a message
        /// </summary>
        /// <param name="context"></param>
        /// <param name="message"></param>
        protected void Fail(ResourceExecutingContext context, string message)
        {
            context.Result = new ContentResult()
            {
                StatusCode = (int)HttpStatusCode.Unauthorized,
                Content = message
            };
        }


    }
}
