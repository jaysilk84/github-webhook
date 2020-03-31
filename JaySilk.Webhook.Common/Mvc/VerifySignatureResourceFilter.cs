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

    // Implemented as a resource filter because it requires access to the http context. The 
    // signature from github is in the header and the body needs to be signed and compared 
    // to the signature. Authorization and Authentication filters don't give you the http
    // context and it's not recommended because if you use IHttpContextAccessor it's not
    // cross platform
    public class VerifySignatureResourceFilter : Attribute, IAsyncResourceFilter
    {
        protected string SignatureHeaderName { get; }
        protected string Secret { get; }

        public VerifySignatureResourceFilter(string signatureHeaderName, string secret)
        {
            SignatureHeaderName = signatureHeaderName;
            Secret = secret;
        }

        protected async Task<bool> OnResourceExecutedAsync(ResourceExecutingContext context)
        {
            if (!context.HttpContext.Request.Headers.ContainsKey(SignatureHeaderName)) {
                Fail(context, "HMAC signature missing");
                return false;

            } else {
                context.HttpContext.Request.EnableBuffering();

                using var reader = new StreamReader(context.HttpContext.Request.Body, encoding: Encoding.UTF8, detectEncodingFromByteOrderMarks: false);
                var signature = (string)context.HttpContext.Request.Headers[SignatureHeaderName];
                string body = await reader.ReadToEndAsync();

                context.HttpContext.Request.Body.Position = 0;

                try {
                    // Convert body to UTF 8 bytes, same with secret since it's just an arbitrary string 
                    if (!IsValid(Encoding.UTF8.GetBytes(body), new HexString(signature.Substring(5)), Encoding.UTF8.GetBytes(Secret))) {
                        Fail(context, "HMAC signature invalid");
                        return false;
                    } else {
                        return true;
                    }

                } catch (FormatException ex) {
                    Fail(context, ex.Message);
                    return false;
                }
            }
        }

        public virtual async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
        {
            if (await OnResourceExecutedAsync(context))
                await next();
        }


        public virtual void OnResourceExecuted(ResourceExecutedContext context)
        {

        }

        /// <summary>
        /// Compute the signature of the passed in body and see if it matches the
        /// expected passed in signature using the passed in key. Key must be the
        /// same key to produce the passed in signature
        /// </summary>
        /// <param name="body"></param>
        /// <param name="signature"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        private bool IsValid(byte[] body, HexString signature, byte[] key)
        {
            // use the key and calculate the signature from the body
            using var hmac = new HMACSHA1(key);
            var hash = hmac.ComputeHash(body);

            return isEqual(signature.GetBytes(), hash);
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

        /// <summary>
        /// Constant time compare of two byte arrays. Doesn't short circut
        /// after an unequal comparison to reduce timing attacks
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private static bool isEqual(IEnumerable<byte> a, IEnumerable<byte> b)
        {
            if (a.Count() != b.Count())
                return false;

            var result = 0;
            for (var i = 0; i < a.Count(); i++)
                result |= a.ElementAt(i) ^ b.ElementAt(i);

            return result == 0;
        }

    }
}
