using Microsoft.Extensions.DependencyInjection;
using GitManager.Interface;
using GitManager.Dto.Issue;
using GitManager.Dto.Epic;
using GitManager.Service.Utility;
using GitManager.Dto.User;

class Program
{
    static async Task Main(string[] args)
    {
        // Configure services
        var services = new ServiceCollection();
        services.AddGitManager(
            gitLabUrl: "https://git.daneshrefah.ir/",
            personalAccessToken: "glpat-z1aYassa7QxGgPCdUJSo"
        );
        var serviceProvider = services.BuildServiceProvider();

        // Resolve service
        var gitService = serviceProvider.GetRequiredService<IGitManagerService>();

        int groupId = 27;
        // Example: Get Git URL for a project (replace with your project ID)
        int projectId = 39;
        // Example: Get an issue


        var test1 = await gitService.Issue.GetAll(299);
        var issue = new IssueDto() { ProjectId = 299, Title = "first Issue" };
        issue.Assignee = new();
        issue.Assignee.Id = 35;
        var test2 = await gitService.Issue.Close(299,11);


        if (test2) {
            Console.WriteLine($"Issues 1 successfully opened."); }
    }
}