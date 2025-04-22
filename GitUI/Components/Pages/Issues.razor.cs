using GitManager.Dto.Issue;
using GitManager.Interface;
using Microsoft.AspNetCore.Components;

namespace GitUI.Components.Pages
{
    public partial class Issues
    {
        [Parameter] public int ProjectId { get; set; }

        public List<IssueDto> IssuesList { get; set; } = new List<IssueDto>();
        protected override async Task OnInitializedAsync()
        {
            //IssuesDto = await IssueService.GetAll(ProjectId);


            ////for Update
            //Issue = await IssueService.Get(ProjectId, IssueIid);
            //Title = Issue.Title;
            //Description = Issue.Description;


            ////for Get
            //Issue = await IssueService.Get(ProjectId, IssueIid);
        }

        private async Task CloseIssue(int issueIid)
        {
            await IssueService.Close(ProjectId, issueIid);
            IssuesList = await IssueService.GetAll(ProjectId);
            StateHasChanged();
        }

        private async Task OpenIssue(int issueIid)
        {
            await IssueService.Open(ProjectId, issueIid);
            IssuesList = await IssueService.GetAll(ProjectId);
            StateHasChanged();
        }


        private string Title { get; set; }
        private string Description { get; set; }

        private async Task CreateIssue()
        {
            var newIssue = await IssueService.Create(ProjectId, Title, Description, 0, 0);
            NavigationManager.NavigateTo($"/issues/{ProjectId}/{newIssue.IssueId}");
        }

        [Parameter] public int IssueIid { get; set; }

        private IssueDto Issue { get; set; }


        private async Task UpdateIssue()
        {
            var updatedIssue = await IssueService.Update(ProjectId, IssueIid, title: Title, description: Description);
            NavigationManager.NavigateTo($"/issues/{ProjectId}/{updatedIssue.IssueId}");
        }


    }
}
