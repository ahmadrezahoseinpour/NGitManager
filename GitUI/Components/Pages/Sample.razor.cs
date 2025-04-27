using GitManager.Dto.Issue;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace GitUI.Components.Pages
{
    public partial class Sample
    {
        [Parameter] public int ProjectId { get; set; }
        [Parameter] public int IssueIid { get; set; }
        private EditContext editContext;

        private string SelectedState { get; set; } = "All";

        private IssueDto IssDto { get; set; } = new(); 
        public List<IssueDto> IssuesList { get; set; } = [];
        public IssueDto Issue2 { get; set; }

        protected override async Task OnInitializedAsync()
        {
            editContext = new EditContext(IssDto);
            IssDto.ProjectId = 0;
            StateHasChanged();
            await base.OnInitializedAsync();
        }
            
        private async Task LoadIssues()
        {
            ProjectId = (int)IssDto.ProjectId;
            if (ProjectId <= 0)
            {
                return;
            }

            try
            {
                // Fetch all issues and filter client-side by state
                var allIssues = await IssueService.GetAll(ProjectId) ?? new List<IssueDto>();
                IssuesList = SelectedState == "All"
                    ? allIssues
                    : allIssues.Where(i => i.State.ToLower() == SelectedState.ToLower()).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching issues: {ex.Message}");
                IssuesList = new List<IssueDto>();
            }
        }
    }
}
