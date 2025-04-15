using NGitLab.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GitManager.Interface
{
    public interface IGitEpic
    {
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
    }
}
