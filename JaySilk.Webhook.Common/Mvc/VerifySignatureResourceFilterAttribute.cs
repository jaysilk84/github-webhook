using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;

namespace JaySilk.Webhook.Common.Mvc
{
    public class VerifySignatureResourceFilterAttribute : Attribute, IAsyncResourceFilter
    {
        private class VerifySignatureResourceFilterWrapper : VerifySignatureResourceFilter {

            public VerifySignatureResourceFilterWrapper(string signatureHeaderName, string secret, Func<string, string> headerValueTransformer)
                : base(signatureHeaderName, secret, headerValueTransformer)
            {
                
            }

            public VerifySignatureResourceFilterWrapper(string signatureHeaderName, string secret)
                : base(signatureHeaderName, secret, Identity) { }
        }

        private readonly VerifySignatureResourceFilterWrapper _wrapper;

        public VerifySignatureResourceFilterAttribute(string signatureHeaderName, string secret, Func<string, string> headerValueTransformer) =>
            _wrapper = new VerifySignatureResourceFilterWrapper(signatureHeaderName, secret, headerValueTransformer); 
        
        public VerifySignatureResourceFilterAttribute(string signatureHeaderName, string secret) 
            : this(signatureHeaderName, secret, x => x) { }
        

        public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
        {
           await _wrapper.OnResourceExecutionAsync(context, next); // forward to wrapper
        }

    }


}
