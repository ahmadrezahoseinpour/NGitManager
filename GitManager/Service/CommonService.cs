using AutoMapper;
using GitManager.Dto.Issue;
using GitManager.Interface;
using NGitLab;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GitManager.Service
{
    public class CommonService : ICommonService
    {
        private readonly GitLabClient _client;
        private readonly IMapper _mapper;
        public CommonService(IMapper mapper, GitLabClient client)
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

        public async Task<string[]> GetLabelsByGroup(int groupId)
        {
            if (groupId <= 0) throw new ArgumentException("group ID must be positive.", nameof(groupId));
            var res = await ExecuteGitLabActionAsync(() => _client.Labels.ForGroup(groupId), $"getting labels for group {groupId}");
            var result = res.Select(p => p.Name).ToArray();
            return result;
        }
        public async Task<string[]> GetLabelsByProject(int projectId)
        {
            if (projectId <= 0) throw new ArgumentException("project ID must be positive.", nameof(projectId));
            var res = await ExecuteGitLabActionAsync(() => _client.Labels.ForProject(projectId), $"getting labels for group {projectId}");
            var result = res.Select(p => p.Name).ToArray();
            return result;
        }

        #endregion

        #region Implementation Milestones

        public async Task<List<MilestoneDto>> GetMilestonesByGroup(int groupId)
        {
            if (groupId <= 0) throw new ArgumentException("group ID must be positive.", nameof(groupId));
            var res = await ExecuteGitLabActionAsync(() => _client.GetGroupMilestone(groupId).All, $"getting milestones for group {groupId}");
            return _mapper.Map<List<MilestoneDto>>(res);
        }
        public async Task<List<MilestoneDto>> GetMilestonesByProject(int projectId)
        {
            if (projectId <= 0) throw new ArgumentException("project ID must be positive.", nameof(projectId));
            var res = await ExecuteGitLabActionAsync(() => _client.GetMilestone(projectId).All, $"getting milestones for group {projectId}");
            return _mapper.Map<List<MilestoneDto>>(res);
        }

        #endregion
    }
}
