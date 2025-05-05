using AutoMapper;
using GitManager.Dto;
using GitManager.Dto.Epic;
using GitManager.Dto.Issue;
using GitManager.Interface;
using NGitLab;
using NGitLab.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GitManager.Service
{
    internal class EpicService : IEpicService
    {

        private readonly GitLabClient _client;
        private readonly IMapper _mapper;
        public EpicService(IMapper mapper, GitLabClient client)
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

        #region Epics Implementation

        public async Task<Response<List<EpicDto>>> GetAll(int groupId, EpicQueryDto query)
        {
            try
            {
                if (groupId <= 0) throw new ArgumentException("Group ID must be positive.", nameof(groupId));
                if (query == null) throw new ArgumentNullException(nameof(query));
                var epicQuery = _mapper.Map<EpicQuery>(query);
                var res = await ExecuteGitLabActionAsync(() => _client.Epics.Get(groupId, epicQuery).ToList(), $"getting epics for group {groupId}");
                if (res.Any())
                {
                    res.ToList();
                    var data = _mapper.Map<List<EpicDto>>(res);
                    return new Response<List<EpicDto>>() { Status = 200, Message = "Succeed", Data = data };
                }
                else return new Response<List<EpicDto>>() { Status = 404, Message = "NotFound", Data = null };
            }
            catch (Exception ex)
            {
                return new Response<List<EpicDto>>() { Status = 400, Message = ex.Message, Data = null };
            }
        }

        public async Task<Response<EpicDto>> Get(int groupId, int epicIid)
        {
            try
            {
                if (groupId <= 0) throw new ArgumentException("Group ID must be positive.", nameof(groupId));
                if (epicIid <= 0) throw new ArgumentException("Epic IID must be positive.", nameof(epicIid));
                var res = await ExecuteGitLabActionAsync(() => _client.Epics.Get(groupId, epicIid), $"getting epic {epicIid} for group {groupId}");
                var data = _mapper.Map<EpicDto>(res);
                return new Response<EpicDto>() { Status = 200, Message = "Succeed", Data = data };
            }
            catch (Exception ex)
            {
                return new Response<EpicDto>() { Status = 400, Message = ex.Message, Data = null };
            }
        }

        public async Task<Response<EpicDto>> Create(EpicDto dto)
        {
            try
            {
                if (dto.GroupId <= 0) throw new ArgumentException("Group ID must be positive.", nameof(dto.GroupId));
                if (string.IsNullOrWhiteSpace(dto.Title)) throw new ArgumentException("Epic title cannot be empty.", nameof(dto.Title));

                var labelStr = "";
                if (dto.Labels != null) labelStr = string.Join(",", dto.Labels);

                var epicCreate = new EpicCreate
                {
                    Title = dto.Title,
                    Description = dto.Description,
                    Labels = labelStr
                };

                var res = await ExecuteGitLabActionAsync(() => _client.Epics.Create(dto.GroupId, epicCreate), $"creating epic in group {dto.GroupId}");
                var data = _mapper.Map<EpicDto>(res);
                return new Response<EpicDto>() { Status = 200, Message = "Succeed", Data = data };
            }
            catch (Exception ex)
            {
                return new Response<EpicDto>() { Status = 400, Message = ex.Message, Data = null };
            }
        }

        public async Task<Response<EpicDto>> Update(EpicDto dto)
        {
            try
            {
                if (dto.GroupId <= 0) throw new ArgumentException("Group ID must be positive.", nameof(dto.GroupId));
                if (dto.EpicIid <= 0) throw new ArgumentException("Epic IID must be positive.", nameof(dto.EpicIid));

                var labelStr = "";
                if (dto.Labels != null && dto.Labels.Any()) labelStr = string.Join(",", dto.Labels);
                else labelStr = null;

                var epicUpdate = new EpicEdit
                {
                    EpicId = dto.EpicIid,
                    Title = dto.Title,
                    Description = dto.Description,
                    State = dto.State.ToString(),
                    Labels = labelStr
                };

                var res = await ExecuteGitLabActionAsync(() => _client.Epics.Edit(dto.GroupId, epicUpdate), $"updating epic {dto.EpicIid} in group {dto.GroupId}");
                var data = _mapper.Map<EpicDto>(res);
                return new Response<EpicDto>() { Status = 200, Message = "Succeed", Data = data };
            }
            catch (Exception ex) {
                return new Response<EpicDto>() { Status = 400, Message = ex.Message, Data = null };
            }
        }

        public async Task<Response<bool>> Close(int groupId, int epicIid)
        {

            try
            {
                var get = await Get(groupId, epicIid);
                EpicDto dto = get.Data;
                if (dto.EpicIid != epicIid)
                {
                    Console.WriteLine("Epic Not Found");
                    return new Response<bool>() { Status = 404, Message = "NotFound", Data = false }; 
                }
                if (dto.State == EpicStateDto.closed)
                {
                    Console.WriteLine($"Epic {epicIid} is already closed");
                    return new Response<bool>() { Status = 409, Message = "Epic is already closed", Data = false };
                }
                dto.State = EpicStateDto.closed;
                var epicUpdate = new EpicEdit
                {
                    EpicId = dto.EpicIid,
                    State = dto.State.ToString(),
                };
                var res = await ExecuteGitLabActionAsync(() => _client.Epics.Edit(groupId, epicUpdate), $"closing epic {epicIid} in group {groupId}");
                return new Response<bool>() { Status = 200, Message = "Succeed", Data = true };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error closing epic {epicIid} in group {groupId}: {ex.Message}");
                return new Response<bool>() { Status = 400, Message = $"Error closing epic {epicIid} in group id {groupId}: {ex.Message}", Data = false };
            }
        }

        public async Task<Response<bool>> Open(int groupId, int epicIid)
        {
            try
            {
                var get = await Get(groupId, epicIid);
                EpicDto dto = get.Data;
                if (dto.EpicIid != epicIid)
                {
                    Console.WriteLine("Epic Not Found");
                    return new Response<bool>() { Status = 404, Message = "NotFound", Data = false };
                }
                if (dto.State == EpicStateDto.opened)
                {
                    Console.WriteLine($"Epic {epicIid} is already opened");
                    return new Response<bool>() { Status = 409, Message = "Epic is already opened", Data = false };
                }
                dto.State = EpicStateDto.opened;
                var epicUpdate = new EpicEdit
                {
                    EpicId = dto.EpicIid,
                    State = dto.State.ToString(),
                };
                var res = await ExecuteGitLabActionAsync(() => _client.Epics.Edit(groupId, epicUpdate), $"opening epic {epicIid} in group id {groupId}");
                return new Response<bool>() { Status = 200, Message = "Succeed", Data = true };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error opening epic {epicIid} in group id {groupId}: {ex.Message}");
                return new Response<bool>() { Status = 400, Message = $"Error closing epic {epicIid} in group id {groupId}: {ex.Message}", Data = false };
            }
        }

        #endregion
    }
}
