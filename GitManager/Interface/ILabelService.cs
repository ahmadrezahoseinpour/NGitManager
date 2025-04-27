using GitManager.Dto.User;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GitManager.Interface
{
    public interface ILabelService
    {
        /// <summary>
        /// Get all for labels based on group id .
        /// </summary>
        /// <param name="groupId">The search term.</param>
        /// <returns>A collection of available labels.</returns>
        /// <exception cref="ArgumentException">Thrown if groupId is null or empty.</exception>
        /// <exception cref="InvalidOperationException">Thrown for unexpected errors during the operation.</exception>
        Task<string[]> GetAllByGroup(int groupId);

        /// <summary>
        /// Get all for labels based on project id .
        /// </summary>
        /// <param name="projectId">The search term.</param>
        /// <returns>A collection of available labels.</returns>
        /// <exception cref="ArgumentException">Thrown if projectId is null or empty.</exception>
        /// <exception cref="InvalidOperationException">Thrown for unexpected errors during the operation.</exception>
        Task<string[]> GetAllByProject(int projectId);
    }
}
