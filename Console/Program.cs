using Microsoft.Extensions.DependencyInjection;
using GitManager.Interface;
using GitManager.Dto.Issue;
using GitManager.Dto.Epic;
using GitManager.Service.Utility;
using GitManager.Dto.Issue.enums;

class Program
{
    static async Task Main(string[] args)
    {
        // Configure services
        var services = new ServiceCollection();
        services.AddGitManager(
            gitLabUrl: "",
            personalAccessToken: ""
        );
        var serviceProvider = services.BuildServiceProvider();

        // Resolve service
        var gitService = serviceProvider.GetRequiredService<IGitManagerService>();

        //int groupId = 27;
        //// Example: Get Git URL for a project (replace with your project ID)
        //int projectId = 39;
        //// Example: Get an issue


        //var test1 = await gitService.Issue.GetAll(299);
        //var issue = new IssueDto() { ProjectId = 299, Title = "first Issue" };
        //issue.Assignee = new();
        //issue.Assignee.Id = 35;

        //var test3 = await gitService.Issue.Open(299, 1);
        //if (test3) { Console.WriteLine("Opening was successful"); }

        //IssueQueryDto qDtop = new()
        //{
        //    State = IssueStateDto.opened
        //};
        //var test1 = await gitService.Issue.Search(190, qDtop);
        //var dto = new IssueDto()
        //{

        //};
        //var res = gitService.Issue.Update(dto);
        //var dto1 = new IssueDto()
        //{
        //    ProjectId = 299,
        //    Title = "Second Issue",
        //};
        //var test = await gitService.Issue.Create(dto1);
        //var test1 = await gitService.Issue.Get(299, 1);

        var dto1 = new IssueDto()
        {
            ProjectId = 299,
            IssueId = 2,
        };
        var dto2 = await gitService.User.GetByUserName("jorsar");
        //Console.WriteLine($"this user is {dto.Name} with username : {dto.Username} and email : {dto.Email}");


    }
}