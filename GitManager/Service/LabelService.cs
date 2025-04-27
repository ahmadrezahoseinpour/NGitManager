using AutoMapper;
using GitManager.Dto.Issue;
using GitManager.Interface;
using GitManager.Service.Utility;
using NGitLab;
using NGitLab.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GitManager.Service
{
    public class LabelService : ILabelService
    {
        private readonly GitLabClient _client;
        private readonly IMapper _mapper;
        public LabelService(IMapper mapper, GitLabClient client)
        {
            _mapper = mapper;
            _client = client;
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
            catch (Exception ex)
            {
                throw new InvalidOperationException($"An unexpected error occurred during '{operationDescription}': {ex.Message}", ex);
            }
        }

        #region Implementation Labels

        public async Task<string[]> GetAllByGroup(int groupId)
        {
            if (groupId <= 0) throw new ArgumentException("group ID must be positive.", nameof(groupId));
            var res = await ExecuteGitLabActionAsync(() => _client.Labels.ForGroup(groupId), $"getting labels for group {groupId}");
            var result = res.Select(p => p.Name).ToArray();
            return result;
        }
        public async Task<string[]> GetAllByProject(int projectId)
        {
            if (projectId <= 0) throw new ArgumentException("project ID must be positive.", nameof(projectId));
            var res = await ExecuteGitLabActionAsync(() => _client.Labels.ForProject(projectId), $"getting labels for group {projectId}");
            var result = res.Select(p => p.Name).ToArray();
            return result;
        }

        #endregion
    }
}
