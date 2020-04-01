using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Builder;

namespace JaySilk.Webhook.Common.Mvc
{

    public static class MvcAppBuilderExtensions
    {
        
       public static IApplicationBuilder UseGitHubSignatureValidation(this IApplicationBuilder builder) =>
           builder.UseMiddleware<GitHubSignatureMiddleware>();

    }


}
