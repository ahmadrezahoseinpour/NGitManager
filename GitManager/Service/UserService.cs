using AutoMapper;
using GitManager.Dto;
using GitManager.Dto.Issue;
using GitManager.Dto.User;
using GitManager.Interface;
using NGitLab;
using NGitLab.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GitManager.Service
{
    internal class UserService : IUserService
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
            catch (GitLabException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                Console.WriteLine("Project not found.");
                throw new InvalidOperationException($"Not found error occurred during '{operationDescription}': {ex.Message}", ex);
            }
            catch (Exception ex) 
            {
                throw new InvalidOperationException($"An unexpected error occurred during '{operationDescription}': {ex.Message}", ex);
            }
        }

        #region User Implementation
        public async Task<Response<UserDto>> GetUserById(int userId)
        {
            try
            {
                if (userId <= 0) throw new ArgumentException("User ID must be positive.", nameof(userId));
                var res = await ExecuteGitLabActionAsync(() => _client.Users.GetByIdAsync(userId), $"getting user with ID '{userId}'").Result;
                if (res.Id == userId)
                {
                    var data = _mapper.Map<UserDto>(res);
                    return new Response<UserDto>() { Status = 200, Message = "Succeed", Data = data };
                }
                else return new Response<UserDto>() { Status = 404, Message = "NotFound", Data = null };
            }
            catch (Exception ex)
            {
                return new Response<UserDto>() { Status = 400, Message = ex.Message, Data = null };
            }
        }

        public async Task<Response<List<UserDto>>> GetByUserName(string username)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(username)) throw new ArgumentException("username cannot be empty.", nameof(username));
                var res = await ExecuteGitLabActionAsync(() => _client.Users.Get(username), $"searching users with string '{username}'");
                if (res.Any())
                {
                    res.ToList();
                    var data = _mapper.Map<List<UserDto>>(res);
                    return new Response<List<UserDto>>() { Status = 200, Message = "Succeed", Data = data };
                }
                else return new Response<List<UserDto>>() { Status = 404, Message = "NotFound", Data = null };
            }
            catch (Exception ex)
            {
                return new Response<List<UserDto>>() { Status = 400, Message = ex.Message, Data = null };
            }
        }

        public async Task<Response<List<UserDto>>> GetAll()
        {
            try
            {
                var query = new UserQuery();
                var res = await ExecuteGitLabActionAsync(() => _client.Users.Get(query), $"searching users with query '{query}'");
                if (res.Any())
                {
                    res.ToList();
                    var data = _mapper.Map<List<UserDto>>(res);
                    return new Response<List<UserDto>>() { Status = 200, Message = "Succeed", Data = data };
                }
                else return new Response<List<UserDto>>() { Status = 404, Message = "NotFound", Data = null };
            }
            catch (Exception ex)
            {
                return new Response<List<UserDto>>() { Status = 400, Message = ex.Message, Data = null };
            }
        }


        public async Task<Response<List<UserDto>>> SearchWithQuery(UserQueryDto userQuery)
        {
            try
            {
                var query = _mapper.Map<UserQuery>(userQuery);

                var res = await ExecuteGitLabActionAsync(() => _client.Users.Get(query), $"searching users with query '{userQuery}'");
                if (res.Any())
                {
                    res.ToList();
                    var data = _mapper.Map<List<UserDto>>(res);
                    return new Response<List<UserDto>>() { Status = 200, Message = "Succeed", Data = data };
                }
                else return new Response<List<UserDto>>() { Status = 404, Message = "NotFound", Data = null };
            }
            catch (Exception ex)
            {
                return new Response<List<UserDto>>() { Status = 400, Message = ex.Message, Data = null };
            }
        }

        #endregion
    }
}
