using GitManager.Dto;
using GitManager.Dto.Epic;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GitManager.Interface
{
    public interface IEpicService
    {
        #region Epics

        /// <summary>
        /// Gets all epics for a specific group.
        /// </summary>
        /// <param name="groupId">The ID or URL-encoded path of the group.</param>
        /// <param name="query">The query parameters to filter epics.</param>
        /// <returns>A collection of epics.</returns>
        /// <exception cref="ArgumentException">Thrown if groupId is invalid.</exception>
        /// <exception cref="InvalidOperationException">Thrown for unexpected errors during the operation.</exception>
        Task<Response<List<EpicDto>>> GetAll(int groupId, EpicQueryDto query);

        ///// <summary>
        ///// Gets epics for a specific group based on a query.
        ///// </summary>
        ///// <param name="groupId">The ID or URL-encoded path of the group.</param>
        ///// <param name="query">The query parameters to filter epics.</param>
        ///// <returns>A collection of epics matching the query.</returns>
        ///// <exception cref="ArgumentException">Thrown if groupId is invalid.</exception>
        ///// <exception cref="InvalidOperationException">Thrown for unexpected errors during the operation.</exception>
        //Task<Response<List<EpicDto>>> Search(int groupId, EpicQueryDto query);

        /// <summary>
        /// Gets a specific epic by its internal ID (Iid) within a group.
        /// </summary>
        /// <param name="groupId">The ID or URL-encoded path of the group.</param>
        /// <param name="epicIid">The internal ID (Iid) of the epic.</param>
        /// <returns>The requested epic.</returns>
        /// <exception cref="ArgumentException">Thrown if groupId or epicIid is invalid.</exception>
        /// <exception cref="InvalidOperationException">Thrown for unexpected errors during the operation.</exception>
        Task<Response<EpicDto>> Get(int groupId, int epicIid);

        /// <summary>
        /// Creates a new epic in a group.
        /// </summary>
        /// <returns>The created epic.</returns>
        /// <exception cref="ArgumentException">Thrown if required parameters are invalid.</exception>
        /// <exception cref="InvalidOperationException">Thrown for unexpected errors during the operation.</exception>
        Task<Response<EpicDto>> Create(EpicDto dto);

        /// <summary>
        /// Updates an existing epic.
        /// </summary>
        /// <returns>The updated epic.</returns>
        /// <exception cref="ArgumentException">Thrown if identifying parameters are invalid.</exception>
        /// <exception cref="InvalidOperationException">Thrown for unexpected errors during the operation.</exception>
        Task<Response<EpicDto>> Update(EpicDto dto);

        /// <summary>
        /// Closes an existing epic.
        /// </summary>
        /// <param name="groupId">The ID or URL-encoded path of the group.</param>
        /// <param name="epicIid">The internal ID (Iid) of the epic.</param>
        /// <returns>The closed epic.</returns>
        /// <exception cref="ArgumentException">Thrown if groupId or epicIid is invalid.</exception>
        /// <exception cref="InvalidOperationException">Thrown for unexpected errors during the operation.</exception>
        Task<Response<bool>> Close(int groupId, int epicIid);

        /// <summary>
        /// Opens an existing epic.
        /// </summary>
        /// <param name="groupId">The ID or URL-encoded path of the group.</param>
        /// <param name="epicIid">The internal ID (Iid) of the epic.</param>
        /// <returns>The closed epic.</returns>
        /// <exception cref="ArgumentException">Thrown if groupId or epicIid is invalid.</exception>
        /// <exception cref="InvalidOperationException">Thrown for unexpected errors during the operation.</exception>
        Task<Response<bool>> Open(int groupId, int epicIid);

        #endregion
    }
}
