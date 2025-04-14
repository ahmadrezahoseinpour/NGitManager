using NGitLab.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GitManager
{
    public interface IGitManagerService
    {

        /// <summary>
        /// Gets all issues for a project
        /// </summary>
        /// <param name="projectId">The ID of the project</param>
        /// <returns>A collection of issues</returns>
        Task<IEnumerable<Issue>> GetIssuesAsync(int projectId);

        /// <summary>
        /// Gets a specific issue by ID
        /// </summary>
        /// <param name="projectId">The ID of the project</param>
        /// <param name="issueId">The ID of the issue</param>
        /// <returns>The requested issue</returns>
        Task<Issue> GetIssueAsync(int projectId, int issueId);

        /// <summary>
        /// Creates a new issue
        /// </summary>
        /// <param name="projectId">The ID of the project</param>
        /// <param name="title">The title of the issue</param>
        /// <param name="description">The description of the issue</param>
        /// <param name="labels">Optional labels for the issue</param>
        /// <returns>The created issue</returns>
        Task<Issue> CreateIssueAsync(int projectId, string title, string description, string[] labels = null);

        /// <summary>
        /// Updates an existing issue
        /// </summary>
        /// <param name="projectId">The ID of the project</param>
        /// <param name="issueId">The ID of the issue</param>
        /// <param name="title">Optional new title</param>
        /// <param name="description">Optional new description</param>
        /// <param name="state">Optional new state</param>
        /// <param name="assigneeIds">ID of assignees</param>
        /// <returns>The updated issue</returns>
        Task<Issue> UpdateIssueAsync(int projectId, int issueId, long[] assigneeIds = null, string title = null, string description = null, string state = null);

        /// <summary>
        /// Updates The State to Close an existing issue
        /// </summary>
        /// <param name="projectId">The ID of the project</param>
        /// <param name="issueId">The ID of the issue</param>
        /// <returns>The closed/updated issue</returns>
        Task<Issue> CloseIssueAsync(int projectId, int issueId);

    }
}
