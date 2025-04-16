using AutoMapper;
using GitManager.Interface;
using GitManager.Mapper;
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
            if (string.IsNullOrWhiteSpace(gitLabUrl))
                throw new ArgumentNullException(nameof(gitLabUrl));
            if (string.IsNullOrWhiteSpace(personalAccessToken))
                throw new ArgumentNullException(nameof(personalAccessToken));

            //services.AddAutoMapper(typeof(MappingProfile).Assembly);
            services.AddAutoMapper(cfg=>
            cfg.AddProfile<MappingProfile>());

            //services.AddSingleton<IGitLabClient>(GitLabClient.Connect(gitLabUrl, personalAccessToken));
            //services.AddScoped<IGitManagerService, GitManagerService>();
            services.AddSingleton<IGitManagerService>(provider =>
            {
                var mapper = provider.GetRequiredService<IMapper>();
                return new GitManagerService(gitLabUrl, personalAccessToken, mapper);
            });

            return services;
        }
    }
}
