using GitManager.Dto.Issue;
using NGitLab.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GitManager.Interface
{
    public interface IIssueService
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
        Task<List<IssueDto>> GetAll(int projectId);

        /// <summary>
        /// Gets issues for a specific project based on a query.
        /// </summary>
        /// <param name="projectId">The ID or URL-encoded path of the project.</param>
        /// <param name="query">The query parameters to filter issues.</param>
        /// <returns>A collection of issues matching the query.</returns>
        /// <exception cref="ArgumentException">Thrown if projectId is invalid.</exception>
        /// <exception cref="GitLabException">Thrown if the GitLab API returns an error.</exception>
        /// <exception cref="InvalidOperationException">Thrown for unexpected errors during the operation.</exception>
        Task<List<IssueDto>> Search(int projectId, IssueQueryDto query);

        /// <summary>
        /// Gets a specific issue by its internal ID (Iid) within a project.
        /// </summary>
        /// <param name="projectId">The ID or URL-encoded path of the project.</param>
        /// <param name="issueIid">The internal ID (Iid) of the issue.</param>
        /// <returns>The requested issue.</returns>
        /// <exception cref="ArgumentException">Thrown if projectId or issueIid is invalid.</exception>
        /// <exception cref="GitLabException">Thrown if the GitLab API returns an error (e.g., not found).</exception>
        /// <exception cref="InvalidOperationException">Thrown for unexpected errors during the operation.</exception>
        Task<IssueDto> Get(int projectId, int issueIid);

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
        Task<IssueDto> Create(int projectId, string title, string description, int epicId, int weight, IEnumerable<string> labels = null, IEnumerable<long> assigneeIds = null);

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
        Task<IssueDto> Update(int projectId, int issueIid, int epicId = 0, int weight = 0, string title = null, string description = null, string state = null, IEnumerable<string> labels = null, IEnumerable<long> assigneeIds = null);

        /// <summary>
        /// Closes an existing issue.
        /// </summary>
        /// <param name="projectId">The ID or URL-encoded path of the project.</param>
        /// <param name="issueIid">The internal ID (Iid) of the issue to close.</param>
        /// <returns>The closed issue.</returns>
        /// <exception cref="ArgumentException">Thrown if projectId or issueIid is invalid.</exception>
        /// <exception cref="GitLabException">Thrown if the GitLab API returns an error.</exception>
        /// <exception cref="InvalidOperationException">Thrown for unexpected errors during the operation.</exception>
        Task<IssueDto> Close(int projectId, int issueIid);

        /// <summary>
        /// Opens an existing issue.
        /// </summary>
        /// <param name="projectId">The ID or URL-encoded path of the project.</param>
        /// <param name="issueIid">The internal ID (Iid) of the issue to close.</param>
        /// <returns>The closed issue.</returns>
        /// <exception cref="ArgumentException">Thrown if projectId or issueIid is invalid.</exception>
        /// <exception cref="GitLabException">Thrown if the GitLab API returns an error.</exception>
        /// <exception cref="InvalidOperationException">Thrown for unexpected errors during the operation.</exception>
        Task<IssueDto> Open(int projectId, int issueIid);

        #endregion
    }
}
