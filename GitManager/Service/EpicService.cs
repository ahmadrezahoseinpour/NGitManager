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

        //public List<Epic> GetGroup(int groupId)
        //{
        //    if (groupId <= 0) throw new ArgumentException("Group ID must be positive.", nameof(groupId));
        //    var res ExecuteGitLabActionAsync(() => _client.Epics.ForGroup(groupId).ToList(), $"getting epics for group {groupId}").Result;
        //    return _mapper.Map<List<EpicDto>>(res);
        //}

        public Task<List<EpicDto>> GetAll(int groupId, EpicQueryDto query)
        {
            if (groupId <= 0) throw new ArgumentException("Group ID must be positive.", nameof(groupId));
            if (query == null) throw new ArgumentNullException(nameof(query));
            var epicQuery = _mapper.Map<EpicQuery>(query);
            var res = ExecuteGitLabActionAsync(() => _client.Epics.Get(groupId, epicQuery).ToList(), $"getting epics for group {groupId} with query").Result;
            return _mapper.Map<Task<List<EpicDto>>>(res);
        }

        public Task<EpicDto> Get(int groupId, int epicIid)
        {
            if (groupId <= 0) throw new ArgumentException("Group ID must be positive.", nameof(groupId));
            if (epicIid <= 0) throw new ArgumentException("Epic IID must be positive.", nameof(epicIid));
            var res = ExecuteGitLabActionAsync(() => _client.Epics.Get(groupId, epicIid), $"getting epic {epicIid} for group {groupId}").Result;
            return _mapper.Map<Task<EpicDto>>(res);
        }

        public Task<EpicDto> Create(int groupId, string title, string description = null, IEnumerable<string> labels = null)
        {
            if (groupId <= 0) throw new ArgumentException("Group ID must be positive.", nameof(groupId));
            if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Epic title cannot be empty.", nameof(title));

            var epicCreate = new EpicCreate
            {
                Title = title,
                Description = description
            };

            if (labels?.Any() == true)
            {
                epicCreate.Labels = string.Join(",", labels);
            }

            var res = ExecuteGitLabActionAsync(() => _client.Epics.Create(groupId, epicCreate), $"creating epic in group {groupId}").Result;
            return _mapper.Map<Task<EpicDto>>(res);
        }

        public Task<EpicDto> Update(int groupId, int epicIid, string title = null, string description = null, string stateEvent = null, IEnumerable<string> labels = null)
        {
            if (groupId <= 0) throw new ArgumentException("Group ID must be positive.", nameof(groupId));
            if (epicIid <= 0) throw new ArgumentException("Epic IID must be positive.", nameof(epicIid));

            var epicUpdate = new EpicEdit
            {
                EpicId = epicIid, // NGitLab uses EpicId here which corresponds to Iid
                Title = title,
                Description = description,
                State = stateEvent // Map to State property in EpicEdit ("close" or "reopen")
            };

            if (labels != null)
            {
                epicUpdate.Labels = string.Join(",", labels);
            }

            var res = ExecuteGitLabActionAsync(() => _client.Epics.Edit(groupId, epicUpdate), $"updating epic {epicIid} in group {groupId}").Result;
            return _mapper.Map<Task<EpicDto>>(res);
        }

        public Task<EpicDto> Close(int groupId, int epicIid)
        {
            // Use UpdateEpicAsync with the correct state event for closing
            return Update(groupId, epicIid, stateEvent: "close");
        }

        public Task<EpicDto> Open(int groupId, int epicIid)
        {
            // Use UpdateEpicAsync with the correct state event for closing
            return Update(groupId, epicIid, stateEvent: "open");
        }

        #endregion
    }
}
