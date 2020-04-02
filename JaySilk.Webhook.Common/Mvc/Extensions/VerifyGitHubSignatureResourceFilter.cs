using System;
using Microsoft.Extensions.Options;

namespace JaySilk.Webhook.Common.Mvc.Extensions
{
    /// <summary>
    /// Github specific implementation of VerifySignatureResourceFilter. 
    /// </summary>
    public class VerifyGitHubSignatureResourceFilter : VerifySignatureResourceFilter
    {
        private const string DEFAULT_HEADER_NAME = "X-Hub-Signature";
        public VerifyGitHubSignatureResourceFilter(IOptions<SignatureOptions> config) : base(DEFAULT_HEADER_NAME, config.Value.Secret, s => s.Substring(5))
        {

        }

    }
}