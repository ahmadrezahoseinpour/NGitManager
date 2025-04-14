using Microsoft.Extensions.DependencyInjection;
using GitManager;

class Program
{
    static async Task Main(string[] args)
    {
        // Configure services
        var services = new ServiceCollection();
        services.AddGitManager(
            gitLabUrl: "https://git.daneshrefah.ir/",
            personalAccessToken: "glpat-MVX1sHXHmPf8zhtobEYy"
        );
        var serviceProvider = services.BuildServiceProvider();

        // Resolve service
        var gitService = serviceProvider.GetRequiredService<IGitManagerService>();


        // Example: Get Git URL for a project (replace with your project ID)
        int projectId = 190;
        int issueId = 285;
        // Example: Get an issue
        var testGetIssue = await gitService.GetIssueAsync(
            projectId,
            issueId
        );
        // Example: Get a List issues
        var testGetIssues = await gitService.GetIssuesAsync(
            projectId
        );
        string descs = "";
        foreach (var i in testGetIssues)
        {
            descs+= i.Description ;
        }
        Console.WriteLine($"Issue got successfully.{testGetIssue.Description}");
        Console.WriteLine($"Issues got successfully.{descs}");
    }
}