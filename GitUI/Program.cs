using GitUI.Components;
using GitManager.Interface;
using GitManager.Service.Utility;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var gitConfig = builder.Configuration.GetSection("Git");
var gitUrl = gitConfig["Url"];
var personalAccessToken = gitConfig["PersonalAccessToken"];

if (string.IsNullOrEmpty(gitUrl) || string.IsNullOrEmpty(personalAccessToken))
{
    throw new InvalidOperationException("Git URL and PersonalAccessToken must be configured.");
}


builder.Services.AddGitManager(gitUrl, personalAccessToken);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
