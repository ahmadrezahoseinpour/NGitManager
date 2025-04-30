using AutoMapper;
using GitManager.Dto.Epic;
using GitManager.Dto.Issue;
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

            CreateMap<Epic, EpicDto>();
            CreateMap<EpicQuery, EpicQueryDto>();
        }
    }
}
