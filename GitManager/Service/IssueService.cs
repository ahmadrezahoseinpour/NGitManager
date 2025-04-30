using AutoMapper;
using GitManager.Dto.Issue;
using GitManager.Interface;
using NGitLab;
using NGitLab.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
                var res = await Task.Run(action);

                return res;
            }
            catch (GitLabException ex) when (ex.StatusCode == HttpStatusCode.NotFound) {
                Console.WriteLine("Project not found.");
                return default;
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
            var res = await ExecuteGitLabActionAsync(() => _client.Issues.ForProject(projectId), $"getting issues for project {projectId}");
            if (res.Any())
            {
                res.ToList();
                return _mapper.Map<List<IssueDto>>(res);
            }
            else
            {
                return new List<IssueDto>();
            }

        }

        public async Task<IssueDto> Get(int projectId, int issueIid)
        {
            try
            {
                if (projectId <= 0) throw new ArgumentException("Project ID must be positive.", nameof(projectId));
                if (issueIid <= 0) throw new ArgumentException("Issue InternalID must be positive.", nameof(issueIid));
                var res = await ExecuteGitLabActionAsync(() => _client.Issues.Get(projectId, issueIid), $"getting issue {issueIid} for project {projectId}");
                if (res == null)
                {
                    return new IssueDto();
                }
                return _mapper.Map<IssueDto>(res);
            }
            catch (Exception ex)
            {
                return new IssueDto();
            }
        }

        public async Task<List<IssueDto>> Search(int projectId, IssueQueryDto query)
        {
            if (projectId <= 0) throw new ArgumentException("Project ID must be positive.", nameof(projectId));
            if (query == null) throw new ArgumentNullException(nameof(query));
            var issueQuery = _mapper.Map<IssueQuery>(query);
            var res = await ExecuteGitLabActionAsync(() => _client.Issues.Get(projectId, issueQuery), $"getting issues for project {projectId} with query");
            if (res.Any())
            {
                res.ToList();
                return _mapper.Map<List<IssueDto>>(res);
            }
            else
            {
                return new List<IssueDto>();
            }
        }

        public async Task<IssueDto> Create(IssueDto dto)
        {
            if (dto.ProjectId <= 0) throw new ArgumentException("Project ID must be positive.", nameof(dto.ProjectId));
            if (string.IsNullOrWhiteSpace(dto.Title)) throw new ArgumentException("Issue title cannot be empty.", nameof(dto.Title));

            var labelStr = "";
            if (dto.Labels != null)
            {
                labelStr = string.Join(",", dto.Labels);
            }
            var issueCreate = new IssueCreate
            {
                ProjectId = dto.ProjectId,
                Title = dto.Title,
                Description = dto.Description,
                AssigneeId = dto.Assignee?.Id, // Nullable, single assignee
                AssigneeIds = dto.Assignee?.Id != null ? null : dto.Assignees?.Select(x => x.Id).ToArray(), // Use AssigneeIds only if AssigneeId is null
                MileStoneId = dto.Milestone?.Id,
                Labels = labelStr,
                Confidential = dto.Confidential,
                DueDate = dto.DueDate,
                EpicId = dto.Epic?.Id,
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
            //if (dto.Epic.Id <= 0) throw new ArgumentException("Epic ID must be positive.", nameof(dto.Epic.Id));

            var labelStr = "";
            if (dto.Labels != null)
            {
                labelStr = string.Join(",", dto.Labels);
            }
            var issueUpdate = new IssueEdit
            {
                IssueId = dto.IssueId,
                ProjectId = dto.ProjectId,
                Title = dto.Title,
                Description = dto.Description,
                AssigneeId = dto.Assignee?.Id, // Nullable, single assignee
                AssigneeIds = dto.Assignee?.Id != null ? null : dto.Assignees?.Select(x => x.Id).ToArray(), // Use AssigneeIds only if AssigneeId is null
                MilestoneId = dto.Milestone?.Id,
                Labels = labelStr,
                Confidential = dto.Confidential,
                DueDate = dto.DueDate,
                EpicId = dto.Epic?.Id,
                Weight = dto.Weight,
                State = dto.State
            };

            //if (assigneeIds != null)
            //{
            //    issueUpdate.AssigneeIds = assigneeIds.ToArray();
            //}
            var res = await ExecuteGitLabActionAsync(() => _client.Issues.Edit(issueUpdate), $"updating issue {dto.IssueId} in project {dto.ProjectId}");
            return _mapper.Map<IssueDto>(res);
        }

        public async Task<bool> Close(int projectId, int issueIid)
        {
            try
            {
                IssueDto dto = await Get(projectId, issueIid);
                if (dto.IssueId != issueIid|| dto.State == "closed") { return false; }
                dto.State = "close";
                var issueUpdate = new IssueEdit
                {
                    ProjectId = projectId,
                    IssueId = issueIid,
                    State = dto.State
                };
                var res = await ExecuteGitLabActionAsync(() => _client.Issues.Edit(issueUpdate), $"closing issue {issueIid} in project {projectId}");
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> Open(int projectId, int issueIid)
        {
            try
            {
                IssueDto dto = await Get(projectId, issueIid);
                if (dto == null || dto.State == "open") { return false; }
                dto.State = "open";
                var issueUpdate = new IssueEdit
                {
                    ProjectId = projectId,
                    IssueId = issueIid,
                    State = dto.State
                };
                var res = await ExecuteGitLabActionAsync(() => _client.Issues.Edit(issueUpdate), $"opening issue {issueIid} in project {projectId}");
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        #endregion

    }
}
