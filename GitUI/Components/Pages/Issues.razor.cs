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
        public string ErrorMessage { get; set; }
        private string SelectedState { get; set; } = "All";
        public bool IsLoading { get; set; }
        [Inject]
        public ILabelService LabelService { get; set; }
        [Inject]
        public IIssueService IssueService { get; set; }

        private string Title { get; set; }
        private string Description { get; set; }

        private string ModalDisplay { get; set; } = "none;";
        private bool _editDialog { get; set; }
        private string[] Labels { get; set; }
        private IssueDto Issue { get; set; } = new();
        public IssueDto UpdateDto { get; set; } = new();
        public IssueDto AddDto { get; set; } = new();
        public List<IssueDto> IssuesList { get; set; }= [];
        private string SelectedLabels { get; set; }

        protected override async Task OnInitializedAsync()
        {
            //IssuesDto = await IssueService.GetAll(ProjectId);


            ////for Update
            //Title = Issue.Title;
            //Description = Issue.Description;


            ////for Get
            //Issue = await IssueService.Get(ProjectId, IssueIid);
            Issue.ProjectId = 0;
            StateHasChanged();
            base.OnInitialized();
        }
        #endregion

        #region Modals


        private void ShowEditDialog()
        {
            ModalDisplay = "flex;";
            _editDialog = true;
            //ModalClass = "show";
            //ShowBackdrop = true;
            StateHasChanged();
        }
        private void CloseEditDialog()
        {
            ModalDisplay = "none;";
            _editDialog = false;
            //ModalClass = "";
            //ShowBackdrop = false;
            StateHasChanged();
        }
        private async Task SubmitEditDialog()
        {
            await UpdateIssue();
        }
        #endregion

        #region Mothods
        private async Task<List<IssueDto>> GetAllIssue(int ProjectId)
        {
            IssuesList = await IssueService.GetAll(ProjectId);
            StateHasChanged();
            return IssuesList;
        }

        private async Task CloseIssue()
        {
            await IssueService.Close(UpdateDto);
            await GetAllIssue(ProjectId);
            StateHasChanged();
        }

        private async Task OpenIssue()
        {
            await IssueService.Open(UpdateDto);
            await GetAllIssue(ProjectId); 
            StateHasChanged();
        }
        private void OnSelectionChanged(ChangeEventArgs e)
        {
            // For multi-select, e.Value is a string array of the selected values
            if (e.Value is string selectedValues)
            {
                UpdateDto.Labels = string.Join(",", selectedValues);
            }
        }
        private async Task SearchIssues()
        {
            await LoadIssues();
            await GetAllLabels();
        }
        private async Task GetAllLabels()
        {
            Labels = [];
            Labels = await LabelService.GetAllByProject(ProjectId);
        }
        private async Task CreateIssue()
        {
            var newIssue = await IssueService.Create(AddDto);
            AddDto = new();
            //NavigationManager.NavigateTo($"/issues/{ProjectId}/{newIssue.IssueId}");
        }

        private async Task UpdateIssue()
        {
            var updatedIssue = await IssueService.Update(UpdateDto);
            UpdateDto = new();

            //NavigationManager.NavigateTo($"/issues/{ProjectId}/{updatedIssue.IssueId}");
        }
        private async Task LoadIssues()
        {   
            ProjectId = (int)Issue.ProjectId;
            if (ProjectId <= 0)
            {
                IssuesList = [];
                ErrorMessage = "Please enter a valid Project ID.";
                IsLoading = false;
                return;
            }

            IsLoading = true;
            ErrorMessage = string.Empty;
            try
            {
                // Fetch all issues and filter client-side by state

                var allIssues = await IssueService.GetAll(ProjectId) ?? [];
                IssuesList = SelectedState == "All"
                    ? allIssues
                    : allIssues.Where(i => i.State.ToLower() == SelectedState.ToLower()).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching issues: {ex.Message}");
                ErrorMessage = "Failed to load issues. Please try again.";
                IssuesList = [];
            }
            finally
            {
                IsLoading = false;
            }
        }
        #endregion

    }
}
