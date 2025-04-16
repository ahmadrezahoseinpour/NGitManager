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

namespace GitManager
{
    internal class GitManagerService : IGitManagerService
    {
        private readonly GitLabClient _client;
        private readonly IMapper _mapper;

        public IGitUser User { get; }
        public IGitEpic Epic { get; }
        public IGitIssue Issue { get; }

        
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

        #region Issues Implementation

        public List<IssueDto> GetGroupIssue(int projectId)
        {
            if (projectId <= 0) throw new ArgumentException("Project ID must be positive.", nameof(projectId));
            var res = ExecuteGitLabActionAsync(() => _client.Issues.ForProject(projectId).ToList(), $"getting issues for project {projectId}").Result;
            return _mapper.Map<List<IssueDto>>(res);

        }

        public List<IssueDto> GetGroupIssue(int projectId, IssueQuery query)
        {
            if (projectId <= 0) throw new ArgumentException("Project ID must be positive.", nameof(projectId));
            if (query == null) throw new ArgumentNullException(nameof(query));
            var res = ExecuteGitLabActionAsync(() => _client.Issues.Get(projectId, query).ToList(), $"getting issues for project {projectId} with query").Result;
            return _mapper.Map<List<IssueDto>>(res);
        }

        public IssueDto GetIssue(int projectId, int issueIid)
        {
            if (projectId <= 0) throw new ArgumentException("Project ID must be positive.", nameof(projectId));
            if (issueIid <= 0) throw new ArgumentException("Issue InternalID must be positive.", nameof(issueIid));
            var res = ExecuteGitLabActionAsync(() => _client.Issues.Get(projectId, issueIid), $"getting issue {issueIid} for project {projectId}").Result;
            return _mapper.Map<IssueDto>(res);
        }

        public IssueDto CreateIssue(int projectId, string title, string description, int epicId, int weight, IEnumerable<string> labels = null, IEnumerable<long> assigneeIds = null)
        {
            if (projectId <= 0) throw new ArgumentException("Project ID must be positive.", nameof(projectId));
            if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Issue title cannot be empty.", nameof(title));

            var issueCreate = new IssueCreate
            {
                ProjectId = projectId,
                Title = title,
                Description = description, // Can be null or empty
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

            var res=  ExecuteGitLabActionAsync(() => _client.Issues.Create(issueCreate), $"creating issue in project {projectId}").Result;
            return _mapper.Map<IssueDto>(res);
        }

        public IssueDto UpdateIssue(int projectId, int issueIid, int epicId = 0, int weight = 0, string title = null, string description = null, string state = null, IEnumerable<string> labels = null, IEnumerable<long> assigneeIds = null)
        {
            if (projectId <= 0) throw new ArgumentException("Project ID must be positive.", nameof(projectId));
            if (issueIid <= 0) throw new ArgumentException("Issue IID must be positive.", nameof(issueIid));
            if (epicId <= 0) throw new ArgumentException("Epic ID must be positive.", nameof(epicId));

            var issueUpdate = new IssueEdit
            {
                ProjectId = projectId,
                IssueId = issueIid, // NGitLab uses IssueId here which corresponds to Iid
                Title = title, // NGitLab handles nulls internally (won't update if null)
                Description = description,
                State = state, // Should be "close" or "reopen"
                EpicId = epicId,
                Weight = weight

            };

            if (labels != null) // If labels are provided (even empty list), update them
            {
                issueUpdate.Labels = string.Join(",", labels);
            }
            if (assigneeIds != null) // If assigneeIds are provided (even empty list), update them
            {
                issueUpdate.AssigneeIds = assigneeIds.ToArray();
            }
            var res = ExecuteGitLabActionAsync(() => _client.Issues.Edit(issueUpdate), $"updating issue {issueIid} in project {projectId}").Result;
            return _mapper.Map<IssueDto>(res);
        }

        public IssueDto CloseIssue(int projectId, int issueIid)
        {
            // This is just a specific case of UpdateIssueAsync
            return UpdateIssue(projectId, issueIid, state: "close");
        }

        #endregion
        #region Epics Implementation

        //public List<Epic> GetGroup(int groupId)
        //{
        //    if (groupId <= 0) throw new ArgumentException("Group ID must be positive.", nameof(groupId));
        //    var res ExecuteGitLabActionAsync(() => _client.Epics.ForGroup(groupId).ToList(), $"getting epics for group {groupId}").Result;
        //    return _mapper.Map<List<EpicDto>>(res);
        //}

        public List<EpicDto> GetGroupEpic(int groupId, EpicQuery query)
        {
            if (groupId <= 0) throw new ArgumentException("Group ID must be positive.", nameof(groupId));
            if (query == null) throw new ArgumentNullException(nameof(query));
            var res = ExecuteGitLabActionAsync(() => _client.Epics.Get(groupId, query).ToList(), $"getting epics for group {groupId} with query").Result;
            return _mapper.Map<List<EpicDto>>(res);
        }

        public EpicDto GetEpic(int groupId, int epicIid)
        {
            if (groupId <= 0) throw new ArgumentException("Group ID must be positive.", nameof(groupId));
            if (epicIid <= 0) throw new ArgumentException("Epic IID must be positive.", nameof(epicIid));
            var res = ExecuteGitLabActionAsync(() => _client.Epics.Get(groupId, epicIid), $"getting epic {epicIid} for group {groupId}").Result;
            return _mapper.Map<EpicDto>(res);
        }

        public EpicDto CreateEpic(int groupId, string title, string description = null, IEnumerable<string> labels = null)
        {
            if (groupId <= 0) throw new ArgumentException("Group ID must be positive.", nameof(groupId));
            if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Epic title cannot be empty.", nameof(title));

            var epicCreate = new EpicCreate
            {
                Title = title,
                Description = description
            };

            if (labels?.Any() == true)
            {
                epicCreate.Labels = string.Join(",", labels);
            }

            var res = ExecuteGitLabActionAsync(() => _client.Epics.Create(groupId, epicCreate), $"creating epic in group {groupId}").Result;
            return _mapper.Map<EpicDto>(res);
        }

        public EpicDto UpdateEpic(int groupId, int epicIid, string title = null, string description = null, string stateEvent = null, IEnumerable<string> labels = null)
        {
            if (groupId <= 0) throw new ArgumentException("Group ID must be positive.", nameof(groupId));
            if (epicIid <= 0) throw new ArgumentException("Epic IID must be positive.", nameof(epicIid));

            var epicUpdate = new EpicEdit
            {
                EpicId = epicIid, // NGitLab uses EpicId here which corresponds to Iid
                Title = title,
                Description = description,
                State = stateEvent // Map to State property in EpicEdit ("close" or "reopen")
            };

            if (labels != null)
            {
                epicUpdate.Labels = string.Join(",", labels);
            }

            var res = ExecuteGitLabActionAsync(() => _client.Epics.Edit(groupId, epicUpdate), $"updating epic {epicIid} in group {groupId}").Result;
            return _mapper.Map<EpicDto>(res);
        }

        public EpicDto CloseEpic(int groupId, int epicIid)
        {
            // Use UpdateEpicAsync with the correct state event for closing
            return UpdateEpic(groupId, epicIid, stateEvent: "close");
        }

        #endregion

        #region Users Implementation

        //public Task<User> GetUserByIdAsync(int username)
        //{
        //    if (userId <= 0) throw new ArgumentException("User ID must be positive.", nameof(userId));
        //    return ExecuteGitLabActionAsync(() => _client.Users.Get(userId), $"getting user by ID {userId}");
        //}

        public List<UserDto> SearchUsers(string searchQuery)
        {
            if (string.IsNullOrWhiteSpace(searchQuery)) throw new ArgumentException("Search query cannot be empty.", nameof(searchQuery));

            var query = new UserQuery { Search = searchQuery };
            var res = ExecuteGitLabActionAsync(() => _client.Users.Get(query).ToList(), $"searching users with query '{searchQuery}'").Result;
            return _mapper.Map<List<UserDto>>(res);
        }

        #endregion
    }
}
