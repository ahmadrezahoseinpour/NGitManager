using NGitLab.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GitManager.Interface
{
    public interface IGitUser
    {
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
