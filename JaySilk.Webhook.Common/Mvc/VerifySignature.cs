using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Http;
using JaySilk.Webhook.Common.Security;


namespace JaySilk.Webhook.Common.Mvc
{

    public abstract class VerifySignature<TContext>
    {
        protected static Func<string, string> Identity = x => x;
        protected readonly string _secret;
        protected readonly string _signatureHeaderName;
        protected readonly Func<string, string> _headerValueTransformer;

        protected VerifySignature(string signatureHeaderName, string secret, Func<string, string> headerValueTransformer)
        {
            _signatureHeaderName = signatureHeaderName;
            _secret = secret;
            _headerValueTransformer = headerValueTransformer;
        }

        protected VerifySignature(string signatureHeaderName, string secret) : this(signatureHeaderName, secret, Identity) { }

        protected abstract Task FailAsync(TContext context, string message);
        protected abstract Task OnSuccess(TContext context);

        protected virtual async Task<bool> Verify(TContext context, HttpContext httpContext)
        {
            var result = await httpContext.VerifySignatureFromHeaderAsync(_signatureHeaderName, _secret, _headerValueTransformer);
            if (result.IsValid) {
                await OnSuccess(context);
                return true;
         
            } else {
                await FailAsync(context, result.Message);
                return false;
            }
        }
    }


}
