using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Builder;

namespace JaySilk.Webhook.Common.Mvc.Extensions
{

    public static class SignatureServiceCollectionExtensions
    {
        private static IServiceCollection RegisterServices(IServiceCollection services) {
            return services.AddSingleton<VerifyGitHubSignatureResourceFilter>();
        }

        public static IServiceCollection AddGitHubSignatureFiltering(this IServiceCollection services, IConfiguration config) =>
            RegisterServices(services).Configure<SignatureOptions>(config.GetSection(typeof(SignatureOptions).ToString()));


        public static IServiceCollection AddGitHubSignatureFiltering(this IServiceCollection services, Action<SignatureOptions> config) =>
            RegisterServices(services).Configure<SignatureOptions>(config);

    }


}
