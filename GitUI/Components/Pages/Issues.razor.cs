using GitManager.Dto.Issue;
using GitManager.Interface;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace GitUI.Components.Pages
{
    public partial class Issues
    {
        #region Properties
        [Parameter] public int ProjectId { get; set; }
        [Parameter] public int IssueIid { get; set; }
        private EditContext editContext;
        public string ErrorMessage { get; set; }
        private string SelectedState { get; set; } = "All";
        public bool IsLoading { get; set; }

        private string Title { get; set; }
        private string Description { get; set; }


        private IssueDto Issue { get; set; } = new();        
        public List<IssueDto> IssuesList { get; set; }= [];
        protected override async Task OnInitializedAsync()
        {
            //IssuesDto = await IssueService.GetAll(ProjectId);


            ////for Update
            //Issue = await IssueService.Get(ProjectId, IssueIid);
            //Title = Issue.Title;
            //Description = Issue.Description;


            ////for Get
            //Issue = await IssueService.Get(ProjectId, IssueIid);
            editContext = new EditContext(Issue);
            Issue.ProjectId = 0;
            StateHasChanged();
            base.OnInitialized();
        }
        #endregion

        #region Modals

        private bool showDetails = false;

        private void ToggleDetails()
        {
            showDetails = !showDetails;
        }
        #endregion

        #region Mothods
        private async Task<List<IssueDto>> GetAllIssue(int ProjectId)
        {
            IssuesList = await IssueService.GetAll(ProjectId);
            StateHasChanged();
            return IssuesList;
        }

        private async Task CloseIssue(int issueIid)
        {
            await IssueService.Close(ProjectId, issueIid);
            await GetAllIssue(ProjectId);
            StateHasChanged();
        }

        private async Task OpenIssue(int issueIid)
        {
            await IssueService.Open(ProjectId, issueIid);
            await GetAllIssue(ProjectId); 
            StateHasChanged();
        }
        private async Task SearchIssues()
        {
            await LoadIssues();
        }

        private async Task CreateIssue()
        {
            var newIssue = await IssueService.Create(ProjectId, Title, Description, 0, 0);
            NavigationManager.NavigateTo($"/issues/{ProjectId}/{newIssue.IssueId}");
        }

        private async Task UpdateIssue()
        {
            var updatedIssue = await IssueService.Update(ProjectId, IssueIid, title: Title, description: Description);
            NavigationManager.NavigateTo($"/issues/{ProjectId}/{updatedIssue.IssueId}");
        }
        private async Task LoadIssues()
        {   
            ProjectId = (int)Issue.ProjectId;
            if (ProjectId <= 0)
            {
                IssuesList = new List<IssueDto>();
                ErrorMessage = "Please enter a valid Project ID.";
                IsLoading = false;
                return;
            }

            IsLoading = true;
            ErrorMessage = string.Empty;
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
                ErrorMessage = "Failed to load issues. Please try again.";
                IssuesList = new List<IssueDto>();
            }
            finally
            {
                IsLoading = false;
            }
        }
        #endregion

    }
}
