using GitManager.Dto.Issue;
using GitManager.Dto.User;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GitManager.Interface
{
    public interface ICommonService
    {
        /// <summary>
        /// Get all for labels based on group id .
        /// </summary>
        /// <param name="groupId">The search term.</param>
        /// <returns>A collection of available labels.</returns>
        /// <exception cref="ArgumentException">Thrown if groupId is null or empty.</exception>
        /// <exception cref="InvalidOperationException">Thrown for unexpected errors during the operation.</exception>
        Task<string[]> GetLabelsByGroup(int groupId);

        /// <summary>
        /// Get all for labels based on project id .
        /// </summary>
        /// <param name="projectId">The search term.</param>
        /// <returns>A collection of available labels.</returns>
        /// <exception cref="ArgumentException">Thrown if projectId is null or empty.</exception>
        /// <exception cref="InvalidOperationException">Thrown for unexpected errors during the operation.</exception>
        Task<string[]> GetLabelsByProject(int projectId);

        /// <summary>
        /// Get all for milestones based on group id .
        /// </summary>
        /// <param name="groupId">The search term.</param>
        /// <returns>A collection of available milestones.</returns>
        /// <exception cref="ArgumentException">Thrown if groupId is null or empty.</exception>
        /// <exception cref="InvalidOperationException">Thrown for unexpected errors during the operation.</exception>
        Task<List<MilestoneDto>> GetMilestonesByGroup(int groupId);

        /// <summary>
        /// Get all for milestones based on project id .
        /// </summary>
        /// <param name="projectId">The search term.</param>
        /// <returns>A collection of available milestones.</returns>
        /// <exception cref="ArgumentException">Thrown if projectId is null or empty.</exception>
        /// <exception cref="InvalidOperationException">Thrown for unexpected errors during the operation.</exception>
        Task<List<MilestoneDto>> GetMilestonesByProject(int projectId);
    }
}
