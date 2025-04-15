using GitManager.Interface;
using NGitLab;
using NGitLab.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitManager
{
    internal class GitManagerService : IGitManagerService
    {
        private readonly GitLabClient _client;

        public IGitUser GitUser { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public IGitEpic GitEpic { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public IGitIssue GitIssue { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }



        //private readonly Action<GitLabException> _onGitLabError; // Optional error handler callback

        /// <summary>
        /// Initializes a new instance of the <see cref="GitManagerService"/> class.
        /// </summary>
        /// <param name="gitLabUrl">The base URL of the GitLab instance (e.g., "https://gitlab.com").</param>
        /// <param name="personalAccessToken">The personal access token for authentication.</param>
        ///// <param name="onGitLabError">Optional callback action to handle GitLabExceptions.</param>
        public GitManagerService(string gitLabUrl, string personalAccessToken, Action<GitLabException> onGitLabError = null)
        {
            if (string.IsNullOrWhiteSpace(gitLabUrl))
                throw new ArgumentNullException(nameof(gitLabUrl));
            if (string.IsNullOrWhiteSpace(personalAccessToken))
                throw new ArgumentNullException(nameof(personalAccessToken));


            _client = new GitLabClient(gitLabUrl.TrimEnd('/'), personalAccessToken);
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

        public Task<List<Issue>> GetProjectIssuesAsync(int projectId)
        {
            if (projectId <= 0) throw new ArgumentException("Project ID must be positive.", nameof(projectId));
            return ExecuteGitLabActionAsync(() => _client.Issues.ForProject(projectId).ToList(), $"getting issues for project {projectId}");
        }

        public Task<List<Issue>> GetProjectIssuesAsync(int projectId, IssueQuery query)
        {
            if (projectId <= 0) throw new ArgumentException("Project ID must be positive.", nameof(projectId));
            if (query == null) throw new ArgumentNullException(nameof(query));
            return ExecuteGitLabActionAsync(() => _client.Issues.Get(projectId, query).ToList(), $"getting issues for project {projectId} with query");
        }

        public Task<Issue> GetIssueAsync(int projectId, int issueIid)
        {
            if (projectId <= 0) throw new ArgumentException("Project ID must be positive.", nameof(projectId));
            if (issueIid <= 0) throw new ArgumentException("Issue InternalID must be positive.", nameof(issueIid));
            return ExecuteGitLabActionAsync(() => _client.Issues.Get(projectId, issueIid), $"getting issue {issueIid} for project {projectId}");
        }

        public Task<Issue> CreateIssueAsync(int projectId, string title, string description, int epicId, int weight, IEnumerable<string> labels = null, IEnumerable<long> assigneeIds = null)
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

            return ExecuteGitLabActionAsync(() => _client.Issues.Create(issueCreate), $"creating issue in project {projectId}");
        }

        public Task<Issue> UpdateIssueAsync(int projectId, int issueIid, int epicId = 0, int weight = 0, string title = null, string description = null, string state = null, IEnumerable<string> labels = null, IEnumerable<long> assigneeIds = null)
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
            IssueQuery issueQuery = new()
            {
                UpdatedAfter = DateTime.Now,
                UpdatedBefore = DateTime.Now.AddMonths(-3)
            };
            _client.Users.
            return ExecuteGitLabActionAsync(() => _client.Issues.Edit(issueUpdate), $"updating issue {issueIid} in project {projectId}");
        }

        public Task<Issue> CloseIssueAsync(int projectId, int issueIid)
        {
            // This is just a specific case of UpdateIssueAsync
            return UpdateIssueAsync(projectId, issueIid, state: "close");
        }

        #endregion
        #region Epics Implementation

        //public Task<IEnumerable<Epic>> GetGroupEpicsAsync(int groupId)
        //{
        //    if (groupId <= 0) throw new ArgumentException("Group ID must be positive.", nameof(groupId));
        //    return ExecuteGitLabActionAsync(() => _client.Epics.ForGroup(groupId).ToList(), $"getting epics for group {groupId}");
        //}

        public Task<List<Epic>> GetGroupEpicsAsync(int groupId, EpicQuery query)
        {
            if (groupId <= 0) throw new ArgumentException("Group ID must be positive.", nameof(groupId));
            if (query == null) throw new ArgumentNullException(nameof(query));
            return ExecuteGitLabActionAsync(() => _client.Epics.Get(groupId, query).ToList(), $"getting epics for group {groupId} with query");
        }

        public Task<Epic> GetEpicAsync(int groupId, int epicIid)
        {
            if (groupId <= 0) throw new ArgumentException("Group ID must be positive.", nameof(groupId));
            if (epicIid <= 0) throw new ArgumentException("Epic IID must be positive.", nameof(epicIid));
            return ExecuteGitLabActionAsync(() => _client.Epics.Get(groupId, epicIid), $"getting epic {epicIid} for group {groupId}");
        }

        public Task<Epic> CreateEpicAsync(int groupId, string title, string description = null, IEnumerable<string> labels = null)
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

            return ExecuteGitLabActionAsync(() => _client.Epics.Create(groupId, epicCreate), $"creating epic in group {groupId}");
        }

        public Task<Epic> UpdateEpicAsync(int groupId, int epicIid, string title = null, string description = null, string stateEvent = null, IEnumerable<string> labels = null)
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

            return ExecuteGitLabActionAsync(() => _client.Epics.Edit(groupId, epicUpdate), $"updating epic {epicIid} in group {groupId}");
        }

        public Task<Epic> CloseEpicAsync(int groupId, int epicIid)
        {
            // Use UpdateEpicAsync with the correct state event for closing
            return UpdateEpicAsync(groupId, epicIid, stateEvent: "close");
        }

        #endregion

        #region Users Implementation

        //public Task<User> GetUserByIdAsync(int username)
        //{
        //    if (userId <= 0) throw new ArgumentException("User ID must be positive.", nameof(userId));
        //    return ExecuteGitLabActionAsync(() => _client.Users.Get(userId), $"getting user by ID {userId}");
        //}

        public Task<List<User>> SearchUsersAsync(string searchQuery)
        {
            if (string.IsNullOrWhiteSpace(searchQuery)) throw new ArgumentException("Search query cannot be empty.", nameof(searchQuery));

            var query = new UserQuery { Search = searchQuery };
            return ExecuteGitLabActionAsync(() => _client.Users.Get(query).ToList(), $"searching users with query '{searchQuery}'");
        }

        #endregion
    }
}
