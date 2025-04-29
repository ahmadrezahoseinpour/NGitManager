using Microsoft.Extensions.DependencyInjection;
using GitManager.Interface;
using GitManager.Dto.Issue;
using GitManager.Dto.Epic;
using GitManager.Service.Utility;

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
        var testLabel = await gitService.Label.GetLabelsByProject(
            projectId
        );
        // Example: Get a List issues


        //Console.WriteLine($"Issues got successfully.{testGetIssue[0].Author.Name}");
        string text = "";
        foreach(var i in testLabel)
        {
            text += i + " - ";

        }
        Console.WriteLine($"Issues got successfully.{text}");
        Console.WriteLine($"Issues got successfully.{testLabel}");
    }
}