using AutoMapper;
using GitManager.Dto.Epic;
using GitManager.Dto.Issue;
using GitManager.Dto.User;
using GitManager.Interface;
using NGitLab;
using NGitLab.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using GitManager.Service;
using GitManager.Service.Utility;

namespace GitManager
{
    internal class GitManagerService : IGitManagerService
    {
        private readonly GitLabClient _client;
        private readonly IMapper _mapper;

        public IUserService User { get; }
        public IEpicService Epic { get; }
        public IIssueService Issue { get; }
        public ICommonService Common { get; }


        //private readonly Action<GitLabException> _onGitLabError; // Optional error handler callback

        /// <summary>
        /// Initializes a new instance of the <see cref="GitManagerService"/> class.
        /// </summary>
        /// <param name="gitLabUrl">The base URL of the GitLab instance (e.g., "https://gitlab.com").</param>
        /// <param name="personalAccessToken">The personal access token for authentication.</param>
        public GitManagerService(string gitLabUrl, string personalAccessToken, IMapper mapper)
        {
            if (string.IsNullOrWhiteSpace(gitLabUrl))
                throw new ArgumentNullException(nameof(gitLabUrl));
            if (string.IsNullOrWhiteSpace(personalAccessToken))
                throw new ArgumentNullException(nameof(personalAccessToken));

            _client = new GitLabClient(gitLabUrl.TrimEnd('/'), personalAccessToken);
            _mapper = mapper;

            Issue = new IssueService(_mapper, _client);
            User = new UserService(_mapper, _client);
            Epic = new EpicService(_mapper, _client);
            Common = new CommonService(_mapper, _client);
        }


        private async Task<T> ExecuteGitLabActionAsync<T>(Func<T> action, string operationDescription)
        {
            try
            {
                return await Task.Run(action);
            }
            catch (GitLabException ex)
            {
                throw new InvalidOperationException($"GitLab API error during '{operationDescription}': {ex.Message} (StatusCode: {ex.StatusCode})", ex);
            }
            catch (Exception ex) when (!(ex is GitLabException)) // Catch non-GitLab exceptions
            {
                throw new InvalidOperationException($"An unexpected error occurred during '{operationDescription}': {ex.Message}", ex);
            }
        }
    }
}
