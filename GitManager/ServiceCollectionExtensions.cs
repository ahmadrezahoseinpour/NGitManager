using Microsoft.Extensions.DependencyInjection;
using NGitLab;
using System;
using System.Collections.Generic;
using System.Text;

namespace GitManager
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddGitManager(this IServiceCollection services, string gitLabUrl, string personalAccessToken)
        {
            if (string.IsNullOrEmpty(gitLabUrl))
                throw new ArgumentNullException(nameof(gitLabUrl));
            if (string.IsNullOrEmpty(personalAccessToken))
                throw new ArgumentNullException(nameof(personalAccessToken));

            //services.AddSingleton<IGitLabClient>(GitLabClient.Connect(gitLabUrl, personalAccessToken));
            //services.AddScoped<IGitManagerService, GitManagerService>();

            services.AddSingleton<IGitManagerService>(new GitManagerService(gitLabUrl, personalAccessToken));
            return services;
        }
    }
}
