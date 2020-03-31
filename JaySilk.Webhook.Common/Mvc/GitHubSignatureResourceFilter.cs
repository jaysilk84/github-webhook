using System;
using Microsoft.Extensions.Options;

namespace JaySilk.Webhook.Common.Mvc
{
    public class GitHubSignatureResourceFilter : VerifySignatureResourceFilter {
        private const string DEFAULT_HEADER_NAME = "X-Hub-Signature";
        public GitHubSignatureResourceFilter(IOptions<SignatureOptions> config) : base(config.Value.SignatureHeaderName ?? DEFAULT_HEADER_NAME, config.Value.Secret)
        {
            
        }
    }
}