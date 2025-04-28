using AutoMapper;
using GitManager.Dto.Issue;
using GitManager.Interface;
using NGitLab;
using NGitLab.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GitManager.Service
{
    internal class IssueService : IIssueService
    {

        private readonly GitLabClient _client;
        private readonly IMapper _mapper;
        public IssueService(IMapper mapper, GitLabClient client)
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
        #region Issues Implementation

        public async Task<List<IssueDto>> GetAll(int projectId)
        {
            if (projectId <= 0) throw new ArgumentException("Project ID must be positive.", nameof(projectId));
            var res = await ExecuteGitLabActionAsync(() => _client.Issues.ForProject(projectId).ToList(), $"getting issues for project {projectId}");
            return _mapper.Map<List<IssueDto>>(res);

        }

        public async Task<List<IssueDto>> Search(int projectId, IssueQueryDto query)
        {
            if (projectId <= 0) throw new ArgumentException("Project ID must be positive.", nameof(projectId));
            if (query == null) throw new ArgumentNullException(nameof(query));
            var issueQuery = _mapper.Map<IssueQuery>(query);
            var res = await ExecuteGitLabActionAsync(() => _client.Issues.Get(projectId, issueQuery).ToList(), $"getting issues for project {projectId} with query");
            return _mapper.Map<List<IssueDto>>(res);
        }

        public async Task<IssueDto> Get(int projectId, int issueIid)
        {
            if (projectId <= 0) throw new ArgumentException("Project ID must be positive.", nameof(projectId));
            if (issueIid <= 0) throw new ArgumentException("Issue InternalID must be positive.", nameof(issueIid));
            var res = await ExecuteGitLabActionAsync(() => _client.Issues.Get(projectId, issueIid), $"getting issue {issueIid} for project {projectId}");
            return _mapper.Map<IssueDto>(res);
        }

        public async Task<IssueDto> Create(IssueDto dto)
        {
            if (dto.ProjectId <= 0) throw new ArgumentException("Project ID must be positive.", nameof(dto.ProjectId));
            if (string.IsNullOrWhiteSpace(dto.Title)) throw new ArgumentException("Issue title cannot be empty.", nameof(dto.Title));

            var issueCreate = new IssueCreate
            {
                Title = dto.Title,
                Description = dto.Description,
                AssigneeId = dto.AssigneeId,
                AssigneeIds = dto.AssigneeIds,
                MileStoneId = dto.MileStoneId,
                Labels = dto.Labels,
                Confidential = dto.Confidential,
                DueDate = dto.DueDate,
                EpicId = dto.EpicId,
                Weight = dto.Weight
            };

            //if (labels?.Any() == true)
            //{
            //    issueCreate.Labels = string.Join(",", labels);
            //}
            //if (assigneeIds?.Any() == true)
            //{
            //    issueCreate.AssigneeIds = assigneeIds.ToArray();
            //}

            var res = await ExecuteGitLabActionAsync(() => _client.Issues.Create(issueCreate), $"creating issue in project {dto.ProjectId}");
            return _mapper.Map<IssueDto>(res);
        }

        public async Task<IssueDto> Update(IssueDto dto)
        {
            if (dto.ProjectId <= 0) throw new ArgumentException("Project ID must be positive.", nameof(dto.ProjectId));
            if (dto.IssueId <= 0) throw new ArgumentException("Issue IID must be positive.", nameof(dto.IssueId));
            if (dto.EpicId <= 0) throw new ArgumentException("Epic ID must be positive.", nameof(dto.EpicId));

            var issueUpdate = new IssueEdit
            {
                Title = dto.Title,
                Description = dto.Description,
                AssigneeId = dto.AssigneeId,
                AssigneeIds = dto.AssigneeIds,
                MilestoneId = dto.MileStoneId,
                Labels = dto.Labels,
                Confidential = dto.Confidential,
                DueDate = dto.DueDate,
                EpicId = dto.EpicId,
                Weight = dto.Weight
            };

            //if (labels != null)
            //{
            //    issueUpdate.Labels = string.Join(",", labels);
            //}
            //if (assigneeIds != null)
            //{
            //    issueUpdate.AssigneeIds = assigneeIds.ToArray();
            //}
            var res = await ExecuteGitLabActionAsync(() => _client.Issues.Edit(issueUpdate), $"updating issue {dto.IssueId} in project {dto.ProjectId}");
            return _mapper.Map<IssueDto>(res);
        }

        public Task<IssueDto> Close(IssueDto dto)
        {
            dto.State = "close";
            return Update(dto);
        }

        public Task<IssueDto> Open(IssueDto dto)
        {
            dto.State = "open";
            return Update(dto);
        }


        #endregion

    }
}
