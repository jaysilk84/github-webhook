using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Builder;

namespace JaySilk.Webhook.Common.Mvc.Extensions
{

    public static class SignatureAppBuilderExtensions
    {
        
       public static IApplicationBuilder UseGitHubSignatureValidation(this IApplicationBuilder builder) =>
           builder.UseMiddleware<GitHubSignatureMiddleware>();

    }


}
