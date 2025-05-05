using GitManager.Dto;
using GitManager.Dto.User;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GitManager.Interface
{
    public interface IUserService
    {
        #region Users

        /// <summary>
        /// Gets a specific user by their GitLab user ID.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>The requested user.</returns>
        /// <exception cref="ArgumentException">Thrown if userId is invalid.</exception>
        /// <exception cref="InvalidOperationException">Thrown for unexpected errors during the operation.</exception>
        Task<Response<UserDto>> GetUserById(int userId);

        /// <summary>
        /// Get a list of users by their GitLab username.
        /// </summary>
        /// <param name="username">The string username of the users.</param>
        /// <returns>The requested user.</returns>
        /// <exception cref="ArgumentException">Thrown if username is invalid.</exception>
        /// <exception cref="InvalidOperationException">Thrown for unexpected errors during the operation.</exception>
        Task<Response<List<UserDto>>> GetByUserName(string username);

        /// <summary>
        /// Get all users.
        /// </summary>
        /// <returns>The requested user.</returns>
        /// <exception cref="InvalidOperationException">Thrown for unexpected errors during the operation.</exception>
        Task<Response<List<UserDto>>> GetAll();

        #endregion
    }
}
