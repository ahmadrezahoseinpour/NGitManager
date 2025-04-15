using NGitLab.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GitManager
{
    public interface IGitManagerService
    {
        #region Issues

        /// <summary>
        /// Gets all issues for a specific project.
        /// </summary>
        /// <param name="projectId">The ID or URL-encoded path of the project.</param>
        /// <returns>A collection of issues.</returns>
        /// <exception cref="ArgumentException">Thrown if projectId is invalid.</exception>
        /// <exception cref="GitLabException">Thrown if the GitLab API returns an error.</exception>
        /// <exception cref="InvalidOperationException">Thrown for unexpected errors during the operation.</exception>
        Task<List<Issue>> GetProjectIssuesAsync(int projectId);

        /// <summary>
        /// Gets issues for a specific project based on a query.
        /// </summary>
        /// <param name="projectId">The ID or URL-encoded path of the project.</param>
        /// <param name="query">The query parameters to filter issues.</param>
        /// <returns>A collection of issues matching the query.</returns>
        /// <exception cref="ArgumentException">Thrown if projectId is invalid.</exception>
        /// <exception cref="GitLabException">Thrown if the GitLab API returns an error.</exception>
        /// <exception cref="InvalidOperationException">Thrown for unexpected errors during the operation.</exception>
        Task<List<Issue>> GetProjectIssuesAsync(int projectId, IssueQuery query);

        /// <summary>
        /// Gets a specific issue by its internal ID (Iid) within a project.
        /// </summary>
        /// <param name="projectId">The ID or URL-encoded path of the project.</param>
        /// <param name="issueIid">The internal ID (Iid) of the issue.</param>
        /// <returns>The requested issue.</returns>
        /// <exception cref="ArgumentException">Thrown if projectId or issueIid is invalid.</exception>
        /// <exception cref="GitLabException">Thrown if the GitLab API returns an error (e.g., not found).</exception>
        /// <exception cref="InvalidOperationException">Thrown for unexpected errors during the operation.</exception>
        Task<Issue> GetIssueAsync(int projectId, int issueIid);

        /// <summary>
        /// Creates a new issue in a project.
        /// </summary>
        /// <param name="projectId">The ID or URL-encoded path of the project.</param>
        /// <param name="title">The title of the issue.</param>
        /// <param name="description">The description of the issue.</param>
        /// <param name="labels">Optional labels for the issue.</param>
        /// <param name="assigneeIds">Optional IDs of users to assign.</param>
        /// <returns>The created issue.</returns>
        /// <exception cref="ArgumentException">Thrown if required parameters are invalid.</exception>
        /// <exception cref="GitLabException">Thrown if the GitLab API returns an error.</exception>
        /// <exception cref="InvalidOperationException">Thrown for unexpected errors during the operation.</exception>
        Task<Issue> CreateIssueAsync(int projectId, string title, string description, IEnumerable<string> labels = null, IEnumerable<long> assigneeIds = null);

        /// <summary>
        /// Updates an existing issue.
        /// </summary>
        /// <param name="projectId">The ID or URL-encoded path of the project.</param>
        /// <param name="issueIid">The internal ID (Iid) of the issue to update.</param>
        /// <param name="title">Optional new title.</param>
        /// <param name="description">Optional new description.</param>
        /// <param name="state">Optional new state event ("close" or "reopen").</param>
        /// <param name="labels">Optional collection of labels to set (replaces existing labels).</param>
        /// <param name="assigneeIds">Optional collection of user IDs to assign (replaces existing assignees).</param>
        /// <returns>The updated issue.</returns>
        /// <exception cref="ArgumentException">Thrown if identifying parameters are invalid.</exception>
        /// <exception cref="GitLabException">Thrown if the GitLab API returns an error.</exception>
        /// <exception cref="InvalidOperationException">Thrown for unexpected errors during the operation.</exception>
        Task<Issue> UpdateIssueAsync(int projectId, int issueIid, string title = null, string description = null, string state = null, IEnumerable<string> labels = null, IEnumerable<long> assigneeIds = null);

        /// <summary>
        /// Closes an existing issue.
        /// </summary>
        /// <param name="projectId">The ID or URL-encoded path of the project.</param>
        /// <param name="issueIid">The internal ID (Iid) of the issue to close.</param>
        /// <returns>The closed issue.</returns>
        /// <exception cref="ArgumentException">Thrown if projectId or issueIid is invalid.</exception>
        /// <exception cref="GitLabException">Thrown if the GitLab API returns an error.</exception>
        /// <exception cref="InvalidOperationException">Thrown for unexpected errors during the operation.</exception>
        Task<Issue> CloseIssueAsync(int projectId, int issueIid);

        #endregion

        #region Epics

        ///// <summary>
        ///// Gets all epics for a specific group.
        ///// </summary>
        ///// <param name="groupId">The ID or URL-encoded path of the group.</param>
        ///// <returns>A collection of epics.</returns>
        ///// <exception cref="ArgumentException">Thrown if groupId is invalid.</exception>
        ///// <exception cref="GitLabException">Thrown if the GitLab API returns an error.</exception>
        ///// <exception cref="InvalidOperationException">Thrown for unexpected errors during the operation.</exception>
        //Task<IEnumerable<Epic>> GetGroupEpicsAsync(int groupId);

        /// <summary>
        /// Gets epics for a specific group based on a query.
        /// </summary>
        /// <param name="groupId">The ID or URL-encoded path of the group.</param>
        /// <param name="query">The query parameters to filter epics.</param>
        /// <returns>A collection of epics matching the query.</returns>
        /// <exception cref="ArgumentException">Thrown if groupId is invalid.</exception>
        /// <exception cref="GitLabException">Thrown if the GitLab API returns an error.</exception>
        /// <exception cref="InvalidOperationException">Thrown for unexpected errors during the operation.</exception>
        Task<List<Epic>> GetGroupEpicsAsync(int groupId, EpicQuery query);

        /// <summary>
        /// Gets a specific epic by its internal ID (Iid) within a group.
        /// </summary>
        /// <param name="groupId">The ID or URL-encoded path of the group.</param>
        /// <param name="epicIid">The internal ID (Iid) of the epic.</param>
        /// <returns>The requested epic.</returns>
        /// <exception cref="ArgumentException">Thrown if groupId or epicIid is invalid.</exception>
        /// <exception cref="GitLabException">Thrown if the GitLab API returns an error (e.g., not found).</exception>
        /// <exception cref="InvalidOperationException">Thrown for unexpected errors during the operation.</exception>
        Task<Epic> GetEpicAsync(int groupId, int epicIid);

        /// <summary>
        /// Creates a new epic in a group.
        /// </summary>
        /// <param name="groupId">The ID or URL-encoded path of the group.</param>
        /// <param name="title">The title of the epic.</param>
        /// <param name="description">Optional description of the epic.</param>
        /// <param name="labels">Optional labels for the epic.</param>
        /// <returns>The created epic.</returns>
        /// <exception cref="ArgumentException">Thrown if required parameters are invalid.</exception>
        /// <exception cref="GitLabException">Thrown if the GitLab API returns an error.</exception>
        /// <exception cref="InvalidOperationException">Thrown for unexpected errors during the operation.</exception>
        Task<Epic> CreateEpicAsync(int groupId, string title, string description = null, IEnumerable<string> labels = null);

        /// <summary>
        /// Updates an existing epic.
        /// </summary>
        /// <param name="groupId">The ID or URL-encoded path of the group.</param>
        /// <param name="epicIid">The internal ID (Iid) of the epic to update.</param>
        /// <param name="title">Optional new title.</param>
        /// <param name="description">Optional new description.</param>
        /// <param name="stateEvent">Optional state event ("close" or "reopen").</param>
        /// <param name="labels">Optional collection of labels to set (replaces existing labels).</param>
        /// <returns>The updated epic.</returns>
        /// <exception cref="ArgumentException">Thrown if identifying parameters are invalid.</exception>
        /// <exception cref="GitLabException">Thrown if the GitLab API returns an error.</exception>
        /// <exception cref="InvalidOperationException">Thrown for unexpected errors during the operation.</exception>
        Task<Epic> UpdateEpicAsync(int groupId, int epicIid, string title = null, string description = null, string stateEvent = null, IEnumerable<string> labels = null);

        /// <summary>
        /// Closes an existing epic.
        /// </summary>
        /// <param name="groupId">The ID or URL-encoded path of the group.</param>
        /// <param name="epicIid">The internal ID (Iid) of the epic to close.</param>
        /// <returns>The closed epic.</returns>
        /// <exception cref="ArgumentException">Thrown if groupId or epicIid is invalid.</exception>
        /// <exception cref="GitLabException">Thrown if the GitLab API returns an error.</exception>
        /// <exception cref="InvalidOperationException">Thrown for unexpected errors during the operation.</exception>
        Task<Epic> CloseEpicAsync(int groupId, int epicIid);

        #endregion

        #region Users

        ///// <summary>
        ///// Gets a specific user by their GitLab user ID.
        ///// </summary>
        ///// <param name="userId">The ID of the user.</param>
        ///// <returns>The requested user.</returns>
        ///// <exception cref="ArgumentException">Thrown if userId is invalid.</exception>
        ///// <exception cref="GitLabException">Thrown if the GitLab API returns an error (e.g., not found).</exception>
        ///// <exception cref="InvalidOperationException">Thrown for unexpected errors during the operation.</exception>
        //Task<User> GetUserByIdAsync(int userId);

        /// <summary>
        /// Searches for users based on a query string (e.g., username, email).
        /// </summary>
        /// <param name="searchQuery">The search term.</param>
        /// <returns>A collection of users matching the search query.</returns>
        /// <exception cref="ArgumentException">Thrown if searchQuery is null or empty.</exception>
        /// <exception cref="GitLabException">Thrown if the GitLab API returns an error.</exception>
        /// <exception cref="InvalidOperationException">Thrown for unexpected errors during the operation.</exception>
        Task<List<User>> SearchUsersAsync(string searchQuery);

        #endregion
    }
}
