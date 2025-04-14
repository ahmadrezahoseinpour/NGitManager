using NGitLab;
using NGitLab.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;

namespace GitManager
{
    internal class GitManagerService : IGitManagerService
    {
        private readonly GitLabClient _client;

        /// <summary>
        /// Initializes a new instance of the GitLabIssueService class
        /// </summary>
        /// <param name="gitUrl">The URL of the GitLab instance</param>
        /// <param name="privateToken">The private token for authentication</param>
        public GitManagerService(string gitUrl, string privateToken)
        {
            if (string.IsNullOrEmpty(gitUrl))
                throw new ArgumentNullException(nameof(gitUrl));

            if (string.IsNullOrEmpty(privateToken))
                throw new ArgumentNullException(nameof(privateToken));

            _client = new GitLabClient(gitUrl, privateToken);
        }

        /// <inheritdoc/>
        /// 
        /// <summary>
        /// Get a list of issues for the specified project.
        /// </summary>
        public async Task<IEnumerable<Issue>> GetIssuesAsync(int projectId)
        {
            try
            {
                var issueClient = _client.Issues.ForProject(projectId);
                return await Task.FromResult(issueClient);
            }
            catch (GitLabException ex)
            {
                // Handle GitLab-specific errors (e.g., invalid project ID, authentication issues)
                throw new InvalidOperationException($"Failed to getting issues in project {projectId}: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                // Handle unexpected errors (e.g., network issues, null references)
                throw new InvalidOperationException($"An unexpected error occurred while getting issues in project {projectId}.", ex);
            }
        }

        /// <inheritdoc/>
        /// 
        /// <summary>
        /// Return project issues for a given query.
        /// </summary>
        public async Task<IEnumerable<Issue>> GetIssuesAsync(int projectId, IssueQuery issueQuery)
        {
            try
            {
                var issueClient = _client.Issues.Get(projectId, issueQuery);
                return await Task.FromResult(issueClient);
            }
            catch (GitLabException ex)
            {
                // Handle GitLab-specific errors (e.g., invalid project ID, authentication issues)
                throw new InvalidOperationException($"Failed to getting issues in project {projectId}: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                // Handle unexpected errors (e.g., network issues, null references)
                throw new InvalidOperationException($"An unexpected error occurred while getting issues in project {projectId}.", ex);
            }
        }

        /// <inheritdoc/>
        /// 
        /// <summary>
        ///     <para>Return a single issue for a project given project.</para>
        ///     <para>url like GET /projects/:id/issues/:issue_id</para>
        /// </summary>
        public async Task<Issue> GetIssueAsync(int projectId, int issueId)
        {
            try
            {
                var issue = _client.Issues.Get(projectId, issueId);
                return await Task.FromResult(issue);
            }
            catch (GitLabException ex)
            {
                // Handle GitLab-specific errors (e.g., invalid project ID, authentication issues)
                throw new InvalidOperationException($"Failed to get issue in project {projectId}: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                // Handle unexpected errors (e.g., network issues, null references)
                throw new InvalidOperationException($"An unexpected error occurred while getting issue in project {projectId}.", ex);
            }
        }

        /// <inheritdoc/>
        /// 
        /// <summary>
        ///     <para>Creates a new issue for a specified project in GitLab.</para>
        ///     <para>Corresponds to POST /projects/:id/issues</para>
        /// </summary>
        public async Task<Issue> CreateIssueAsync(int projectId, string title, string description, string[] labels = null)
        {
            try
            {

                var issueCreate = new IssueCreate
                {
                    ProjectId = projectId,
                    Title = title,
                    Description = description
                };

                if (labels != null && labels.Length > 0)
                {
                    issueCreate.Labels = string.Join(",", labels);
                }

                var issue = _client.Issues.Create(issueCreate);
                return await Task.FromResult(issue);
            }
            catch (GitLabException ex)
            {
                // Handle GitLab-specific errors (e.g., invalid project ID, authentication issues)
                throw new InvalidOperationException($"Failed to create issue in project {projectId}: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                // Handle unexpected errors (e.g., network issues, null references)
                throw new InvalidOperationException($"An unexpected error occurred while creating issue in project {projectId}.", ex);
            }
        }

        /// <inheritdoc/>
        /// /// <summary>
        ///     <para>Updates the issue for a specified project in GitLab.</para>
        ///     <para>Corresponds to PUT /projects/:id/issues</para>
        /// </summary>
        public async Task<Issue> UpdateIssueAsync(int projectId, int issueId, long[] assigneeIds, string title = null, string description = null, string state = null)
        {
            try
            {
                var issueUpdate = new IssueEdit
                {
                    AssigneeIds = assigneeIds,
                    ProjectId = projectId,
                    IssueId = issueId
                };

                if (!string.IsNullOrEmpty(title))
                    issueUpdate.Title = title;

                if (!string.IsNullOrEmpty(description))
                    issueUpdate.Description = description;

                if (!string.IsNullOrEmpty(state))
                    issueUpdate.State = state;

                var issue = _client.Issues.Edit(issueUpdate);
                return await Task.FromResult(issue);
            }
            catch (GitLabException ex)
            {
                // Handle GitLab-specific errors (e.g., invalid project ID, authentication issues)
                throw new InvalidOperationException($"Failed to update issue in project {projectId}: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                // Handle unexpected errors (e.g., network issues, null references)
                throw new InvalidOperationException($"An unexpected error occurred while updating issue in project {projectId}.", ex);
            }


        }

        /// <summary>
        ///     <para>Close a single issue from a specified project in GitLab.</para>
        ///     <para>Corresponds to EDIT /projects/:id/issues/:issue_iid</para>
        /// </summary>
        /// <param name="projectId">The ID of the project containing the issue.</param>
        /// <param name="issueId">The ID of the issue to delete.</param>
        /// <exception cref="InvalidOperationException">Thrown when the issue deletion fails due to API errors or unexpected issues.</exception>
        public async Task<Issue> CloseIssueAsync(int projectId, int issueId)
        {
            try
            {
                if (projectId <= 0)
                    throw new ArgumentException("Project ID must be a positive integer.", nameof(projectId));
                if (issueId <= 0)
                    throw new ArgumentException("Issue IID must be a positive integer.", nameof(issueId));

                var issueEdit = new IssueEdit
                {
                    ProjectId = projectId,
                    IssueId = issueId,
                    State = "close"
                };
                var updatedIssue = await Task.Run(() => _client.Issues.Edit(issueEdit));
                return updatedIssue;
            }
            catch (GitLabException ex)
            {
                // Handle GitLab-specific errors (e.g., invalid project ID, issue not found, permissions)
                throw new InvalidOperationException($"Failed to delete issue {issueId} in project {projectId}: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                // Handle unexpected errors (e.g., network issues, null references)
                throw new InvalidOperationException($"An unexpected error occurred while deleting issue {issueId} in project {projectId}.", ex);
            }
        }
    }
}