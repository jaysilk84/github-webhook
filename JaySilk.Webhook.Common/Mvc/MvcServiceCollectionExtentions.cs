using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace JaySilk.Webhook.Common.Mvc
{

    public static class MvcServiceCollectionExtentions
    {
        
        public static void AddGitHubSignatureFiltering(this IServiceCollection services, IConfiguration config) {
            services.Configure<SignatureOptions>(config.GetSection(typeof(SignatureOptions).ToString()));
            services.AddSingleton<GitHubSignatureResourceFilter>();
        }

    }


}
