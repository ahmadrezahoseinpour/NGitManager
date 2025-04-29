using AutoMapper;
using GitManager.Dto.Epic;
using GitManager.Interface;
using NGitLab;
using NGitLab.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitManager.Service
{
    internal class EpicService :IEpicService
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
            catch (GitLabException ex)
            {
                throw new InvalidOperationException($"GitLab API error during '{operationDescription}': {ex.Message} (StatusCode: {ex.StatusCode})", ex);
            }
            catch (Exception ex) when (!(ex is GitLabException)) // Catch non-GitLab exceptions
            {
                throw new InvalidOperationException($"An unexpected error occurred during '{operationDescription}': {ex.Message}", ex);
            }
        }

        #region Epics Implementation

        //public async Task<List<Epic>> GetAll(int groupId)
        //{
        //    if (groupId <= 0) throw new ArgumentException("Group ID must be positive.", nameof(groupId));
        //    var res = await ExecuteGitLabActionAsync(() => _client.Epics.G(groupId).ToList(), $"getting epics for group {groupId}");
        //    return _mapper.Map<List<EpicDto>>(res);
        //}

        public async Task<List<EpicDto>> Search(int groupId, EpicQueryDto query)
        {
            if (groupId <= 0) throw new ArgumentException("Group ID must be positive.", nameof(groupId));
            if (query == null) throw new ArgumentNullException(nameof(query));
            var epicQuery = _mapper.Map<EpicQuery>(query);
            var res = await ExecuteGitLabActionAsync(() => _client.Epics.Get(groupId, epicQuery).ToList(), $"getting epics for group {groupId} with query");
            return _mapper.Map<List<EpicDto>>(res);
        }

        public async Task<EpicDto> GetById(int groupId, int epicIid)
        {
            if (groupId <= 0) throw new ArgumentException("Group ID must be positive.", nameof(groupId));
            if (epicIid <= 0) throw new ArgumentException("Epic IID must be positive.", nameof(epicIid));
            var res = await ExecuteGitLabActionAsync(() => _client.Epics.Get(groupId, epicIid), $"getting epic {epicIid} for group {groupId}");
            return _mapper.Map<EpicDto>(res);
        }

        public async Task<EpicDto> Create(EpicDto dto)
        {
            if (dto.GroupId <= 0) throw new ArgumentException("Group ID must be positive.", nameof(dto.GroupId));
            if (string.IsNullOrWhiteSpace(dto.Title)) throw new ArgumentException("Epic title cannot be empty.", nameof(dto.Title));

            var epicCreate = new EpicCreate
            {
                Title = dto.Title,
                Description = dto.Description,
                Labels = string.Join(",", dto.Labels)
            };

            //if (labels?.Any() == true)
            //{
            //    epicCreate.Labels = string.Join(",", labels);
            //}

            var res = await ExecuteGitLabActionAsync(() => _client.Epics.Create(dto.GroupId, epicCreate), $"creating epic in group {dto.GroupId}");
            return _mapper.Map<EpicDto>(res);
        }

        public async Task<EpicDto> Update(EpicDto dto)
        {
            if (dto.GroupId<= 0) throw new ArgumentException("Group ID must be positive.", nameof(dto.GroupId));
            if (dto.EpicIid<= 0) throw new ArgumentException("Epic IID must be positive.", nameof(dto.EpicIid));

            var epicUpdate = new EpicEdit
            {
                EpicId = dto.EpicIid,
                Title = dto.Title,
                Description = dto.Description,
                State = dto.State.ToString(),
                Labels = string.Join(",", dto.Labels)
            };

            //if (labels != null)
            //{
            //    epicUpdate.Labels = string.Join(",", labels);
            //}

            var res = await ExecuteGitLabActionAsync(() => _client.Epics.Edit(dto.GroupId, epicUpdate), $"updating epic {dto.EpicIid} in group {dto.GroupId}");
            return _mapper.Map<EpicDto>(res);
        }

        public Task<EpicDto> Close(EpicDto dto)
        {
            dto.State = EpicState.closed;
            // Use UpdateEpicAsync with the correct state event for closing
            return Update(dto);
        }

        public Task<EpicDto> Open(EpicDto dto)
        {
            dto.State = EpicState.opened;
            return Update(dto);
        }

        #endregion
    }
}
