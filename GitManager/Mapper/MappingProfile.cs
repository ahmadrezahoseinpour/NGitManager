using AutoMapper;
using GitManager.Dto.Epic;
using GitManager.Dto.Issue;
using GitManager.Dto.Issue.enums;
using GitManager.Dto.User;
using NGitLab;
using NGitLab.Models;

namespace GitManager.Mapper
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDto>();
            CreateMap<Identity, IdentityDto>();
            CreateMap<UserQuery, UserQueryDto>();

            CreateMap<Issue, IssueDto>();
            CreateMap<Author, AuthorDto>();
            CreateMap<Assignee, AssigneeDto>();
            CreateMap<IssueEpic, IssueEpicDto>();
            CreateMap<Milestone, MilestoneDto>();
            CreateMap<References, ReferencesDto>();
            CreateMap<IssueQuery, IssueQueryDto>();
            CreateMap<IssueState, IssueStateDto>();
            CreateMap<IssueType, IssueTypeDto>();
            CreateMap<QueryAssigneeId, QueryAssigneeIdDto>();

            CreateMap<Epic, EpicDto>();
            CreateMap<EpicQuery, EpicQueryDto>();
            CreateMap<EpicQueryDto, EpicQuery>(); // Reverse map 
            CreateMap<EpicState, EpicStateDto>();

            CreateMap<IssueStateDto, IssueState>(); // Reverse map 
            CreateMap<IssueTypeDto, IssueType>();   // Reverse map
            CreateMap<QueryAssigneeIdDto, QueryAssigneeId>(); // *** NEED THIS DIRECTION ***
            CreateMap<IssueQueryDto, IssueQuery>(); // *** NEED THIS MAIN MAP ***

        }
    }
}
