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

            // Consider adding more robust URL validation if needed
            _client = new GitLabClient(gitLabUrl.TrimEnd('/'), personalAccessToken); // Ensure no trailing slash
            //_onGitLabError = onGitLabError;
        }

        // Helper to wrap NGitLab calls with error handling
        private async Task<T> ExecuteGitLabActionAsync<T>(Func<T> action, string operationDescription)
        {
            try
            {
                // NGitLab calls are often synchronous, use Task.Run to avoid blocking async methods
                return await Task.Run(action);
            }
            catch (GitLabException ex)
            {
                //_onGitLabError?.Invoke(ex); // Invoke optional handler
                // Provide more context in the exception
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
            // Use ToList() to ensure enumeration happens within the Task.Run context
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

        public Task<Issue> CreateIssueAsync(int projectId, string title, string description, IEnumerable<string> labels = null, IEnumerable<long> assigneeIds = null)
        {
            if (projectId <= 0) throw new ArgumentException("Project ID must be positive.", nameof(projectId));
            if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Issue title cannot be empty.", nameof(title));

            var issueCreate = new IssueCreate
            {
                ProjectId = projectId,
                Title = title,
                Description = description // Can be null or empty
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

        public Task<Issue> UpdateIssueAsync(int projectId, int issueIid, string title = null, string description = null, string state = null, IEnumerable<string> labels = null, IEnumerable<long> assigneeIds = null)
        {
            if (projectId <= 0) throw new ArgumentException("Project ID must be positive.", nameof(projectId));
            if (issueIid <= 0) throw new ArgumentException("Issue IID must be positive.", nameof(issueIid));

            var issueUpdate = new IssueEdit
            {
                ProjectId = projectId,
                IssueId = issueIid, // NGitLab uses IssueId here which corresponds to Iid
                Title = title, // NGitLab handles nulls internally (won't update if null)
                Description = description,
                State = state // Should be "close" or "reopen"
            };

            if (labels != null) // If labels are provided (even empty list), update them
            {
                issueUpdate.Labels = string.Join(",", labels);
            }
            if (assigneeIds != null) // If assigneeIds are provided (even empty list), update them
            {
                issueUpdate.AssigneeIds = assigneeIds.ToArray();
            }

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

            return ExecuteGitLabActionAsync(() => _client.Epics.Create(groupId,epicCreate), $"creating epic in group {groupId}");
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


        //#region Issues
        ///// <inheritdoc/>
        ///// 
        ///// <summary>
        ///// Get a list of issues for the specified project.
        ///// </summary>
        //public async Task<IEnumerable<Issue>> GetIssuesAsync(int projectId)
        //{
        //    try
        //    {
        //        var issueClient = _client.Issues.ForProject(projectId);
        //        return await Task.FromResult(issueClient);
        //    }
        //    catch (GitLabException ex)
        //    {
        //        // Handle GitLab-specific errors (e.g., invalid project ID, authentication issues)
        //        throw new InvalidOperationException($"Failed to getting issues in project {projectId}: {ex.Message}", ex);
        //    }
        //    catch (Exception ex)
        //    {
        //        // Handle unexpected errors (e.g., network issues, null references)
        //        throw new InvalidOperationException($"An unexpected error occurred while getting issues in project {projectId}.", ex);
        //    }
        //}

        ///// <inheritdoc/>
        ///// 
        ///// <summary>
        ///// Return project issues for a given query.
        ///// </summary>
        //public async Task<IEnumerable<Issue>> GetIssuesAsync(int projectId, IssueQuery issueQuery)
        //{
        //    try
        //    {
        //        var issueClient = _client.Issues.Get(projectId, issueQuery);
        //        return await Task.FromResult(issueClient);
        //    }
        //    catch (GitLabException ex)
        //    {
        //        // Handle GitLab-specific errors (e.g., invalid project ID, authentication issues)
        //        throw new InvalidOperationException($"Failed to getting issues in project {projectId}: {ex.Message}", ex);
        //    }
        //    catch (Exception ex)
        //    {
        //        // Handle unexpected errors (e.g., network issues, null references)
        //        throw new InvalidOperationException($"An unexpected error occurred while getting issues in project {projectId}.", ex);
        //    }
        //}

        ///// <inheritdoc/>
        ///// 
        ///// <summary>
        /////     <para>Return a single issue for a project given project.</para>
        /////     <para>url like GET /projects/:id/issues/:issue_id</para>
        ///// </summary>
        //public async Task<Issue> GetIssueAsync(int projectId, int issueId)
        //{
        //    try
        //    {
        //        var issue = _client.Issues.Get(projectId, issueId);
        //        return await Task.FromResult(issue);
        //    }
        //    catch (GitLabException ex)
        //    {
        //        // Handle GitLab-specific errors (e.g., invalid project ID, authentication issues)
        //        throw new InvalidOperationException($"Failed to get issue in project {projectId}: {ex.Message}", ex);
        //    }
        //    catch (Exception ex)
        //    {
        //        // Handle unexpected errors (e.g., network issues, null references)
        //        throw new InvalidOperationException($"An unexpected error occurred while getting issue in project {projectId}.", ex);
        //    }
        //}

        ///// <inheritdoc/>
        ///// 
        ///// <summary>
        /////     <para>Creates a new issue for a specified project in GitLab.</para>
        /////     <para>Corresponds to POST /projects/:id/issues</para>
        ///// </summary>
        //public async Task<Issue> CreateIssueAsync(int projectId, string title, string description, string[] labels = null)
        //{
        //    try
        //    {

        //        var issueCreate = new IssueCreate
        //        {
        //            ProjectId = projectId,
        //            Title = title,
        //            Description = description
        //        };

        //        if (labels != null && labels.Length > 0)
        //        {
        //            issueCreate.Labels = string.Join(",", labels);
        //        }

        //        var issue = _client.Issues.Create(issueCreate);
        //        return await Task.FromResult(issue);
        //    }
        //    catch (GitLabException ex)
        //    {
        //        // Handle GitLab-specific errors (e.g., invalid project ID, authentication issues)
        //        throw new InvalidOperationException($"Failed to create issue in project {projectId}: {ex.Message}", ex);
        //    }
        //    catch (Exception ex)
        //    {
        //        // Handle unexpected errors (e.g., network issues, null references)
        //        throw new InvalidOperationException($"An unexpected error occurred while creating issue in project {projectId}.", ex);
        //    }
        //}

        ///// <inheritdoc/>
        ///// /// <summary>
        /////     <para>Updates the issue for a specified project in GitLab.</para>
        /////     <para>Corresponds to PUT /projects/:id/issues</para>
        ///// </summary>
        //public async Task<Issue> UpdateIssueAsync(int projectId, int issueId, long[] assigneeIds, string title = null, string description = null, string state = null)
        //{
        //    try
        //    {
        //        var issueUpdate = new IssueEdit
        //        {
        //            AssigneeIds = assigneeIds,
        //            ProjectId = projectId,
        //            IssueId = issueId
        //        };

        //        if (!string.IsNullOrEmpty(title))
        //            issueUpdate.Title = title;

        //        if (!string.IsNullOrEmpty(description))
        //            issueUpdate.Description = description;

        //        if (!string.IsNullOrEmpty(state))
        //            issueUpdate.State = state;

        //        var issue = _client.Issues.Edit(issueUpdate);
        //        return await Task.FromResult(issue);
        //    }
        //    catch (GitLabException ex)
        //    {
        //        // Handle GitLab-specific errors (e.g., invalid project ID, authentication issues)
        //        throw new InvalidOperationException($"Failed to update issue in project {projectId}: {ex.Message}", ex);
        //    }
        //    catch (Exception ex)
        //    {
        //        // Handle unexpected errors (e.g., network issues, null references)
        //        throw new InvalidOperationException($"An unexpected error occurred while updating issue in project {projectId}.", ex);
        //    }


        //}

        ///// <summary>
        /////     <para>Close a single issue from a specified project in GitLab.</para>
        /////     <para>Corresponds to EDIT /projects/:id/issues/:issue_iid</para>
        ///// </summary>
        ///// <param name="projectId">The ID of the project containing the issue.</param>
        ///// <param name="issueId">The ID of the issue to delete.</param>
        ///// <exception cref="InvalidOperationException">Thrown when the issue deletion fails due to API errors or unexpected issues.</exception>
        //public async Task<Issue> CloseIssueAsync(int projectId, int issueId)
        //{
        //    try
        //    {
        //        if (projectId <= 0)
        //            throw new ArgumentException("Project ID must be a positive integer.", nameof(projectId));
        //        if (issueId <= 0)
        //            throw new ArgumentException("Issue IID must be a positive integer.", nameof(issueId));

        //        var issueEdit = new IssueEdit
        //        {
        //            ProjectId = projectId,
        //            IssueId = issueId,
        //            State = "close"
        //        };
        //        var updatedIssue = await Task.Run(() => _client.Issues.Edit(issueEdit));
        //        return updatedIssue;
        //    }
        //    catch (GitLabException ex)
        //    {
        //        // Handle GitLab-specific errors (e.g., invalid project ID, issue not found, permissions)
        //        throw new InvalidOperationException($"Failed to delete issue {issueId} in project {projectId}: {ex.Message}", ex);
        //    }
        //    catch (Exception ex)
        //    {
        //        // Handle unexpected errors (e.g., network issues, null references)
        //        throw new InvalidOperationException($"An unexpected error occurred while deleting issue {issueId} in project {projectId}.", ex);
        //    }
        //}
        //#endregion


        //#region Epic
        ///// <inheritdoc/>
        ///// 
        ///// <summary>
        ///// Get a list of epic for the specified project.
        ///// </summary>
        //public async Task<IEnumerable<Issue>> GetIssuesAsync(int projectId)
        //{
        //    try
        //    {
        //        var issueClient = _client.Issues.ForProject(projectId);
        //        return await Task.FromResult(issueClient);
        //    }
        //    catch (GitLabException ex)
        //    {
        //        // Handle GitLab-specific errors (e.g., invalid project ID, authentication issues)
        //        throw new InvalidOperationException($"Failed to getting issues in project {projectId}: {ex.Message}", ex);
        //    }
        //    catch (Exception ex)
        //    {
        //        // Handle unexpected errors (e.g., network issues, null references)
        //        throw new InvalidOperationException($"An unexpected error occurred while getting issues in project {projectId}.", ex);
        //    }
        //}

        ///// <inheritdoc/>
        ///// 
        ///// <summary>
        ///// Return project issues for a given query.
        ///// </summary>
        //public async Task<IEnumerable<Issue>> GetIssuesAsync(int projectId, IssueQuery issueQuery)
        //{
        //    try
        //    {
        //        var issueClient = _client.Issues.Get(projectId, issueQuery);
        //        return await Task.FromResult(issueClient);
        //    }
        //    catch (GitLabException ex)
        //    {
        //        // Handle GitLab-specific errors (e.g., invalid project ID, authentication issues)
        //        throw new InvalidOperationException($"Failed to getting issues in project {projectId}: {ex.Message}", ex);
        //    }
        //    catch (Exception ex)
        //    {
        //        // Handle unexpected errors (e.g., network issues, null references)
        //        throw new InvalidOperationException($"An unexpected error occurred while getting issues in project {projectId}.", ex);
        //    }
        //}

        ///// <inheritdoc/>
        ///// 
        ///// <summary>
        /////     <para>Return a single issue for a project given project.</para>
        /////     <para>url like GET /projects/:id/issues/:issue_id</para>
        ///// </summary>
        //public async Task<Issue> GetIssueAsync(int projectId, int issueId)
        //{
        //    try
        //    {
        //        var issue = _client.Issues.Get(projectId, issueId);
        //        return await Task.FromResult(issue);
        //    }
        //    catch (GitLabException ex)
        //    {
        //        // Handle GitLab-specific errors (e.g., invalid project ID, authentication issues)
        //        throw new InvalidOperationException($"Failed to get issue in project {projectId}: {ex.Message}", ex);
        //    }
        //    catch (Exception ex)
        //    {
        //        // Handle unexpected errors (e.g., network issues, null references)
        //        throw new InvalidOperationException($"An unexpected error occurred while getting issue in project {projectId}.", ex);
        //    }
        //}

        ///// <inheritdoc/>
        ///// 
        ///// <summary>
        /////     <para>Creates a new issue for a specified project in GitLab.</para>
        /////     <para>Corresponds to POST /projects/:id/issues</para>
        ///// </summary>
        //public async Task<Issue> CreateIssueAsync(int projectId, string title, string description, string[] labels = null)
        //{
        //    try
        //    {

        //        var issueCreate = new IssueCreate
        //        {
        //            ProjectId = projectId,
        //            Title = title,
        //            Description = description
        //        };

        //        if (labels != null && labels.Length > 0)
        //        {
        //            issueCreate.Labels = string.Join(",", labels);
        //        }

        //        var issue = _client.Issues.Create(issueCreate);
        //        return await Task.FromResult(issue);
        //    }
        //    catch (GitLabException ex)
        //    {
        //        // Handle GitLab-specific errors (e.g., invalid project ID, authentication issues)
        //        throw new InvalidOperationException($"Failed to create issue in project {projectId}: {ex.Message}", ex);
        //    }
        //    catch (Exception ex)
        //    {
        //        // Handle unexpected errors (e.g., network issues, null references)
        //        throw new InvalidOperationException($"An unexpected error occurred while creating issue in project {projectId}.", ex);
        //    }
        //}

        ///// <inheritdoc/>
        ///// /// <summary>
        /////     <para>Updates the issue for a specified project in GitLab.</para>
        /////     <para>Corresponds to PUT /projects/:id/issues</para>
        ///// </summary>
        //public async Task<Issue> UpdateIssueAsync(int projectId, int issueId, long[] assigneeIds, string title = null, string description = null, string state = null)
        //{
        //    try
        //    {
        //        var issueUpdate = new IssueEdit
        //        {
        //            AssigneeIds = assigneeIds,
        //            ProjectId = projectId,
        //            IssueId = issueId
        //        };

        //        if (!string.IsNullOrEmpty(title))
        //            issueUpdate.Title = title;

        //        if (!string.IsNullOrEmpty(description))
        //            issueUpdate.Description = description;

        //        if (!string.IsNullOrEmpty(state))
        //            issueUpdate.State = state;

        //        var issue = _client.Issues.Edit(issueUpdate);
        //        return await Task.FromResult(issue);
        //    }
        //    catch (GitLabException ex)
        //    {
        //        // Handle GitLab-specific errors (e.g., invalid project ID, authentication issues)
        //        throw new InvalidOperationException($"Failed to update issue in project {projectId}: {ex.Message}", ex);
        //    }
        //    catch (Exception ex)
        //    {
        //        // Handle unexpected errors (e.g., network issues, null references)
        //        throw new InvalidOperationException($"An unexpected error occurred while updating issue in project {projectId}.", ex);
        //    }


        //}

        ///// <summary>
        /////     <para>Close a single issue from a specified project in GitLab.</para>
        /////     <para>Corresponds to EDIT /projects/:id/issues/:issue_iid</para>
        ///// </summary>
        ///// <param name="projectId">The ID of the project containing the issue.</param>
        ///// <param name="issueId">The ID of the issue to delete.</param>
        ///// <exception cref="InvalidOperationException">Thrown when the issue deletion fails due to API errors or unexpected issues.</exception>
        //public async Task<Issue> CloseIssueAsync(int projectId, int issueId)
        //{
        //    try
        //    {
        //        if (projectId <= 0)
        //            throw new ArgumentException("Project ID must be a positive integer.", nameof(projectId));
        //        if (issueId <= 0)
        //            throw new ArgumentException("Issue IID must be a positive integer.", nameof(issueId));

        //        var issueEdit = new IssueEdit
        //        {
        //            ProjectId = projectId,
        //            IssueId = issueId,
        //            State = "close"
        //        };
        //        var updatedIssue = await Task.Run(() => _client.Issues.Edit(issueEdit));
        //        return updatedIssue;
        //    }
        //    catch (GitLabException ex)
        //    {
        //        // Handle GitLab-specific errors (e.g., invalid project ID, issue not found, permissions)
        //        throw new InvalidOperationException($"Failed to delete issue {issueId} in project {projectId}: {ex.Message}", ex);
        //    }
        //    catch (Exception ex)
        //    {
        //        // Handle unexpected errors (e.g., network issues, null references)
        //        throw new InvalidOperationException($"An unexpected error occurred while deleting issue {issueId} in project {projectId}.", ex);
        //    }
        //}
        //#endregion
    }
}
