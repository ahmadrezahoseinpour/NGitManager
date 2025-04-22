using AutoMapper;
using GitManager.Dto.Issue;
using GitManager.Interface;
using NGitLab;
using NGitLab.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitManager.Service
{
    internal class IssueService:IIssueService
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

        public Task<List<IssueDto>> GetAll(int projectId)
        {
            if (projectId <= 0) throw new ArgumentException("Project ID must be positive.", nameof(projectId));
            var res = ExecuteGitLabActionAsync(() => _client.Issues.ForProject(projectId).ToList(), $"getting issues for project {projectId}").Result;
            return _mapper.Map<Task<List<IssueDto>>>(res);

        }

        public Task<List<IssueDto>> Search(int projectId, IssueQueryDto query)
        {
            if (projectId <= 0) throw new ArgumentException("Project ID must be positive.", nameof(projectId));
            if (query == null) throw new ArgumentNullException(nameof(query));
            var issueQuery = _mapper.Map<IssueQuery>(query); 
            var res = ExecuteGitLabActionAsync(() => _client.Issues.Get(projectId, issueQuery).ToList(), $"getting issues for project {projectId} with query").Result;
            return _mapper.Map<Task<List<IssueDto>>>(res);
        }

        public Task<IssueDto> Get(int projectId, int issueIid)
        {
            if (projectId <= 0) throw new ArgumentException("Project ID must be positive.", nameof(projectId));
            if (issueIid <= 0) throw new ArgumentException("Issue InternalID must be positive.", nameof(issueIid));
            var res = ExecuteGitLabActionAsync(() => _client.Issues.Get(projectId, issueIid), $"getting issue {issueIid} for project {projectId}").Result;
            return _mapper.Map<Task<IssueDto>>(res);
        }

        public Task<IssueDto> Create(int projectId, string title, string description, int epicId, int weight, IEnumerable<string> labels = null, IEnumerable<long> assigneeIds = null)
        {
            if (projectId <= 0) throw new ArgumentException("Project ID must be positive.", nameof(projectId));
            if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Issue title cannot be empty.", nameof(title));

            var issueCreate = new IssueCreate
            {
                ProjectId = projectId,
                Title = title,
                Description = description,
                EpicId = epicId,
                Weight = weight

            };

            if (labels?.Any() == true)
            {
                issueCreate.Labels = string.Join(",", labels);
            }
            if (assigneeIds?.Any() == true)
            {
                issueCreate.AssigneeIds = assigneeIds.ToArray();
            }

            var res = ExecuteGitLabActionAsync(() => _client.Issues.Create(issueCreate), $"creating issue in project {projectId}").Result;
            return _mapper.Map<Task<IssueDto>>(res);
        }

        public Task<IssueDto> Update(int projectId, int issueIid, int epicId = 0, int weight = 0, string title = null, string description = null, string state = null, IEnumerable<string> labels = null, IEnumerable<long> assigneeIds = null)
        {
            if (projectId <= 0) throw new ArgumentException("Project ID must be positive.", nameof(projectId));
            if (issueIid <= 0) throw new ArgumentException("Issue IID must be positive.", nameof(issueIid));
            if (epicId <= 0) throw new ArgumentException("Epic ID must be positive.", nameof(epicId));

            var issueUpdate = new IssueEdit
            {
                ProjectId = projectId,
                IssueId = issueIid, 
                Title = title, 
                Description = description,
                State = state, 
                EpicId = epicId,
                Weight = weight

            };

            if (labels != null)
            {
                issueUpdate.Labels = string.Join(",", labels);
            }
            if (assigneeIds != null)
            {
                issueUpdate.AssigneeIds = assigneeIds.ToArray();
            }
            var res = ExecuteGitLabActionAsync(() => _client.Issues.Edit(issueUpdate), $"updating issue {issueIid} in project {projectId}").Result;
            return _mapper.Map<Task<IssueDto>>(res);
        }

        public Task<IssueDto> Close(int projectId, int issueIid)
        {
            return Update(projectId, issueIid, state: "close");
        }

        public Task<IssueDto> Open(int projectId, int issueIid)
        {
            return Update(projectId, issueIid, state: "open");
        }

        #endregion
    }
}
