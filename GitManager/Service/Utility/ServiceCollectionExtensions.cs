using AutoMapper;
using GitManager.Interface;
using GitManager.Mapper;
using GitManager.Service;
using Microsoft.Extensions.DependencyInjection;
using NGitLab;
using System;
using System.Collections.Generic;
using System.Text;

namespace GitManager.Service.Utility
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddGitManager(this IServiceCollection services, string gitLabUrl, string personalAccessToken)
        {
            if (string.IsNullOrWhiteSpace(gitLabUrl))
                throw new ArgumentNullException(nameof(gitLabUrl));
            if (string.IsNullOrWhiteSpace(personalAccessToken))
                throw new ArgumentNullException(nameof(personalAccessToken));


            GitLabClient _client;
        //services.AddAutoMapper(typeof(MappingProfile).Assembly);
        services.AddAutoMapper(cfg =>
            cfg.AddProfile<MappingProfile>());

            var gitLabClient = new GitLabClient(gitLabUrl, personalAccessToken);

            //services.AddSingleton<IGitLabClient>(GitLabClient.Connect(gitLabUrl, personalAccessToken));
            //services.AddScoped<IGitManagerService, GitManagerService>();

            services.AddSingleton<IGitManagerService, GitManagerService>(provider =>
            {
                var mapper = provider.GetRequiredService<IMapper>();
                return new GitManagerService(gitLabUrl, personalAccessToken, mapper);
            });
            services.AddSingleton<IIssueService, IssueService>(provider =>
            {
                var mapper = provider.GetRequiredService<IMapper>();
                return new IssueService(mapper, gitLabClient);
            });
            services.AddSingleton<ICommonService, CommonService>(provider =>
            {
                var mapper = provider.GetRequiredService<IMapper>();
                return new CommonService(mapper, gitLabClient);
            });
            services.AddSingleton<IUserService, UserService>(provider =>
            {
                var mapper = provider.GetRequiredService<IMapper>();
                return new UserService(mapper, gitLabClient);
            });
            services.AddSingleton<IEpicService, EpicService>(provider =>
            {
                var mapper = provider.GetRequiredService<IMapper>();
                return new EpicService(mapper, gitLabClient);
            });

            return services;
        }
}
}
