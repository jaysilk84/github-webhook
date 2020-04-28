using System;
using System.Collections.Generic;
using System.Linq;
using JaySilk.Webhook.Common.Mvc;

namespace JaySilk.Webhook.Common.Mvc.Extensions
{

    public class VerifyGitHubSignatureAttribute : VerifySignatureResourceFilterAttribute 
    {
        
        public VerifyGitHubSignatureAttribute(string secret) : base("X-Hub-Signature", secret, x => x.Substring(5))
        {

        }

    }


}
