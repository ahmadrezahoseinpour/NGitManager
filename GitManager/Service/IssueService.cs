using AutoMapper;
using GitManager.Dto;
using GitManager.Dto.Issue;
using GitManager.Dto.User;
using GitManager.Interface;
using NGitLab;
using NGitLab.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GitManager.Service
{
    internal class IssueService : IIssueService
    {

        private readonly GitLabClient _client;
        private readonly IMapper _mapper;
        public IssueService(IMapper mapper, GitLabClient client)
        {
            _mapper = mapper;
            _client = client;
        }


        private async Task<T> ExecuteGitLabActionAsync<T>(Func<T> action, string operationDescription)
        {
            try
            {
                var res = await Task.Run(action);
                return res;
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
        #region Issues Implementation


        public async Task<Response<List<IssueDto>>> GetAll(int projectId)
        {
            try
            {
                if (projectId <= 0) throw new ArgumentException("Project ID must be positive.", nameof(projectId));
                var res = await ExecuteGitLabActionAsync(() => _client.Issues.ForProject(projectId), $"getting issues for project {projectId}");
                if (res.Any())
                {
                    res.ToList();
                    var data = _mapper.Map<List<IssueDto>>(res);
                    return new Response<List<IssueDto>>() { Status = 200, Message = "Succeed", Data = data };
                }
                else return new Response<List<IssueDto>>() { Status = 404, Message = "NotFound", Data = null };
            }
            catch (Exception ex) {
                return new Response<List<IssueDto>>() { Status = 400, Message = ex.Message, Data = null };
            }

        }

        public async Task<Response<IssueDto>> Get(int projectId, int issueIid)
        {
            try
            {
                if (projectId <= 0) throw new ArgumentException("Project ID must be positive.", nameof(projectId));
                if (issueIid <= 0) throw new ArgumentException("Issue InternalID must be positive.", nameof(issueIid));
                var res = await ExecuteGitLabActionAsync(() => _client.Issues.Get(projectId, issueIid), $"getting issue {issueIid} for project {projectId}");
                if (issueIid == res.IssueId)
                {
                    var data = _mapper.Map<IssueDto>(res);
                    return new Response<IssueDto>() { Status = 200, Message = "Succeed", Data = data };
                }
                else return new Response<IssueDto>() { Status = 404, Message = "NotFound", Data = null };
            }
            catch (Exception ex) { 
                return new Response<IssueDto>() { Status = 400, Message = ex.Message, Data = null }; 
            }
        }

        public async Task<Response<List<IssueDto>>> Search(int projectId, IssueQueryDto query)
        {
            try
            {
                if (projectId <= 0) throw new ArgumentException("Project ID must be positive.", nameof(projectId));
                if (query == null) throw new ArgumentNullException(nameof(query));
                var issueQuery = _mapper.Map<IssueQuery>(query);
                var res = await ExecuteGitLabActionAsync(() => _client.Issues.Get(projectId, issueQuery), $"getting issues for project {projectId} with query");
                if (res.Any())
                {
                    res.ToList();
                    var data = _mapper.Map<List<IssueDto>>(res);
                    return new Response<List<IssueDto>>() { Status = 200, Message = "Succeed", Data = data };
                }
                else return new Response<List<IssueDto>>() { Status = 404, Message = "NotFound", Data = null };
            }
            catch (Exception ex) { 
                return new Response<List<IssueDto>>() { Status = 400, Message = ex.Message, Data = null }; 
            }
        }

        public async Task<Response<IssueDto>> Create(IssueDto dto)
        {
            try
            {
                if (dto.ProjectId <= 0) throw new ArgumentException("Project ID must be positive.", nameof(dto.ProjectId));
                if (string.IsNullOrWhiteSpace(dto.Title)) throw new ArgumentException("Issue title cannot be empty.", nameof(dto.Title));

                var labelStr = "";
                if (dto.Labels != null) labelStr = string.Join(",", dto.Labels);

                long[] targetAssigneeIds = null;
                bool assigneesProvidedInDto = (dto.Assignees != null && dto.Assignees.Any()) || (dto.Assignee?.Id != null);

                if (assigneesProvidedInDto)
                {
                    if (dto.Assignees != null && dto.Assignees.Any())
                    {
                        targetAssigneeIds = dto.Assignees.Select(x => x.Id).ToArray();
                    }
                    else targetAssigneeIds = new long[] { dto.Assignee.Id };
                }

                var issueCreate = new IssueCreate
                {
                    ProjectId = dto.ProjectId,
                    Title = dto.Title,
                    Description = dto.Description,
                    AssigneeId = null,
                    AssigneeIds = targetAssigneeIds,
                    //AssigneeIds = dto.Assignee?.Id != null ? null : dto.Assignees?.Select(x => x.Id).ToArray(), // Use AssigneeIds only if AssigneeId is null
                    MileStoneId = dto.Milestone?.Id,
                    Labels = labelStr,
                    Confidential = dto.Confidential,
                    DueDate = dto.DueDate,
                    EpicId = dto.Epic?.Id,
                    Weight = dto.Weight
                };

                var res = await ExecuteGitLabActionAsync(() => _client.Issues.Create(issueCreate), $"creating issue in project {dto.ProjectId}");
                var data = _mapper.Map<IssueDto>(res);
                return new Response<IssueDto>() { Status = 200, Message = "Succeed", Data = data };
            }
            catch (Exception ex) { 
                return new Response<IssueDto>() { Status = 400, Message = ex.Message, Data = null }; 
            }
        }

        public async Task<Response<IssueDto>> Update(IssueDto dto)
        {
            try
            {
                if (dto.ProjectId <= 0) throw new ArgumentException("Project ID must be positive.", nameof(dto.ProjectId));
                if (dto.IssueId <= 0) throw new ArgumentException("Issue IID must be positive.", nameof(dto.IssueId));
                //if (dto.Epic.Id <= 0) throw new ArgumentException("Epic ID must be positive.", nameof(dto.Epic.Id));

                var labelStr = "";
                if (dto.Labels != null && dto.Labels.Any())
                {
                    labelStr = string.Join(",", dto.Labels);
                }
                else labelStr = null;

                long[] targetAssigneeIds = null;
                bool assigneesProvidedInDto = (dto.Assignees != null && dto.Assignees.Any()) || (dto.Assignee?.Id != null);

                if (assigneesProvidedInDto)
                {
                    if (dto.Assignees != null && dto.Assignees.Any())
                    {
                        targetAssigneeIds = dto.Assignees.Select(x => x.Id).ToArray();
                    }
                    else targetAssigneeIds = new long[] { dto.Assignee.Id };
                }

                var issueUpdate = new IssueEdit
                {
                    IssueId = dto.IssueId,
                    ProjectId = dto.ProjectId,
                    Title = dto.Title,
                    Description = dto.Description,
                    AssigneeId = null,
                    AssigneeIds = targetAssigneeIds,
                    //AssigneeIds = dto.Assignee?.Id != null ? null : dto.Assignees?.Select(x => x.Id).ToArray(), // Use AssigneeIds only if AssigneeId is null
                    MilestoneId = dto.Milestone?.Id,
                    Labels = labelStr,
                    Confidential = dto.Confidential,
                    DueDate = dto.DueDate,
                    EpicId = dto.Epic?.Id,
                    Weight = dto.Weight,
                    State = dto.State
                };

                var res = await ExecuteGitLabActionAsync(() => _client.Issues.Edit(issueUpdate), $"updating issue {dto.IssueId} in project {dto.ProjectId}");
                var data = _mapper.Map<IssueDto>(res);
                return new Response<IssueDto>() { Status = 200, Message = "Succeed", Data = data };
            }
            catch (Exception ex) { return new Response<IssueDto>() { Status = 400, Message = ex.Message, Data = null }; }
        }

        public async Task<Response<bool>> Close(int projectId, int issueIid)
        {
            try
            {
                var get = await Get(projectId, issueIid);
                IssueDto dto = get.Data;
                if (dto.IssueId != issueIid)
                {
                    Console.WriteLine("Issue Not Found");
                    return  new Response<bool>() { Status = 404, Message = "NotFound", Data = false }; ;
                }
                if (dto.State == "closed")
                {
                    Console.WriteLine($"Issue {issueIid} is already closed");
                    return new Response<bool>() { Status = 409, Message = "Issue is already closed", Data = false};
                }
                dto.State = "close";
                var issueUpdate = new IssueEdit
                {
                    ProjectId = projectId,
                    IssueId = issueIid,
                    State = dto.State
                };
                var res = await ExecuteGitLabActionAsync(() => _client.Issues.Edit(issueUpdate), $"closing issue {issueIid} in project id {projectId}");
                return new Response<bool>() { Status = 200, Message = "Succeed", Data = true };
                }
            catch (Exception ex)
            {
                Console.WriteLine($"Error closing issue {issueIid} in project {projectId}: {ex.Message}");
                return new Response<bool>() { Status = 400, Message = $"Error closing issue {issueIid} in project id {projectId}: {ex.Message}", Data = false };
            }
        }

        public async Task<Response<bool>> Open(int projectId, int issueIid)
        {
            try
            {
                var get = await Get(projectId, issueIid);
                IssueDto dto = get.Data;
                if (dto.IssueId != issueIid)
                {
                    Console.WriteLine("Issue Not Found");
                    return new Response<bool>() { Status = 404, Message = "NotFound", Data = false }; ;
                }
                if (dto.State == "opened")
                {
                    Console.WriteLine($"Issue {issueIid} is already opened");
                    return new Response<bool>() { Status = 409, Message = "Issue is already opened", Data = false };
                }
                dto.State = "reopen";
                var issueUpdate = new IssueEdit
                {
                    ProjectId = projectId,
                    IssueId = issueIid,
                    State = dto.State
                };
                var res = await ExecuteGitLabActionAsync(() => _client.Issues.Edit(issueUpdate), $"opening issue {issueIid} in project id {projectId}");
                return new Response<bool>() { Status = 200, Message = "Succeed", Data = true };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error opening issue {issueIid} in project id {projectId}: {ex.Message}");
                return new Response<bool>() { Status = 400, Message = $"Error closing issue {issueIid} in project id {projectId}: {ex.Message}", Data = false };
            }
        }

        #endregion

    }
}
