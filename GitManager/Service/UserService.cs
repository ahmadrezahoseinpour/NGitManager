using AutoMapper;
using GitManager.Dto.User;
using GitManager.Interface;
using NGitLab;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitManager.Service
{
    internal class UserService:IUserService
    {

        private readonly GitLabClient _client;
        private readonly IMapper _mapper;
        public UserService(IMapper mapper, GitLabClient client)
        {
            _mapper = mapper;
            _client = client;
        }


        private async Task<T> ExecuteGitLabActionAsync<T>(Func<T> action, string operationDescription)
        {
            try
            {
                return await Task.Run(action);
            }
            catch (GitLabException ex)
            {
                throw new InvalidOperationException($"GitLab API error during '{operationDescription}': {ex.Message} (StatusCode: {ex.StatusCode})", ex);
            }
            catch (Exception ex) when (!(ex is GitLabException)) // Catch non-GitLab exceptions
            {
                throw new InvalidOperationException($"An unexpected error occurred during '{operationDescription}': {ex.Message}", ex);
            }
        }

        #region Users Implementation

        //public Task<User> GetUserByIdAsync(int username)
        //{
        //    if (userId <= 0) throw new ArgumentException("User ID must be positive.", nameof(userId));
        //    return ExecuteGitLabActionAsync(() => _client.Users.Get(userId), $"getting user by ID {userId}");
        //}


        public List<UserDto> Search(string searchQuery)
        {
            if (string.IsNullOrWhiteSpace(searchQuery)) throw new ArgumentException("Search query cannot be empty.", nameof(searchQuery));

            var query = new UserQuery { Search = searchQuery };
            var res = ExecuteGitLabActionAsync(() => _client.Users.Get(query).ToList(), $"searching users with query '{searchQuery}'").Result;
            return _mapper.Map<List<UserDto>>(res);
        }

        #endregion
    }
}
