using GitManager.Dto.Issue;
using GitManager.Dto.User;
using GitManager.Interface;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using NGitLab.Models;
using System.Linq.Expressions;

namespace GitUI.Components.Pages
{
    public partial class Issues
    {
        #region Properties

        [Inject]
        public ICommonService CommonService { get; set; }
        [Inject]
        public IIssueService IssueService { get; set; }
        [Parameter] public int ProjectId { get; set; }
        [Parameter] public int IssueIid { get; set; }
        public string ErrorMessage { get; set; }
        private string SelectedState { get; set; } = "All";
        private string Title { get; set; }
        private string Description { get; set; }

        private string ModalDisplay { get; set; } = "none;";
        private bool _editDialog { get; set; }
        private string[] Labels { get; set; }
        private string[] Milestones { get; set; }
        public IssueDto Issue { get; set; } = new();
        public IssueDto UpdateDto { get; set; } = new();
        public IssueDto AddDto { get; set; } = new();
        public MilestoneDto Milestone { get; set; }
        public List<IssueDto> IssuesList { get; set; } = [];
        private string SelectedLabels { get; set; }
        private string SelectedMilestone { get; set; } = "";

        private bool IsLoading = false; 
        private bool HasSearched = false;


        protected override async Task OnInitializedAsync()
        {
            //IssuesDto = await IssueService.GetAll(ProjectId);


            ////for Update
            //Title = Issue.Title;
            //Description = Issue.Description;


            ////for Get
            //Issue = await IssueService.Get(ProjectId, IssueIid);
            Issue.ProjectId = 0;
            Issue.Milestone = new MilestoneDto();
            Issue.Assignee = new AssigneeDto();
            
            Issue.Author= new AuthorDto();
            Issue.ClosedBy = new UserDto();
            Issue.Epic = new IssueEpicDto();
            Issue.References = new ReferencesDto();

            StateHasChanged();
            await base.OnInitializedAsync();
        }
        #endregion

        #region Modals


        private void ShowEditDialog(IssueDto dto)
        {

            SelectedLabels = string.Join(",", dto.Labels);
            ModalDisplay = "flex;";
            _editDialog = true;
            StateHasChanged();
        }
        private void CloseEditDialog()
        {
            ModalDisplay = "none;";
            _editDialog = false;
            StateHasChanged();
        }
        private async Task SubmitEditDialog()
        {
            Issue.Milestone.Title = SelectedMilestone;
            await UpdateIssue();
        }
        #endregion

        #region Mothods
        private async Task<List<IssueDto>> GetAllIssue(int ProjectId)
        {
            try
            {
                IsLoading = true;
                HasSearched = true;

                IssuesList = await IssueService.GetAll(ProjectId);
                StateHasChanged();
                return IssuesList;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching issues: {ex.Message}");
                return new List<IssueDto>();
            }
            finally
            {

                IsLoading = false;
            }
        }

        private async Task CloseIssue()
        {
            try
            {
                await IssueService.Close(UpdateDto);
                await GetAllIssue(ProjectId);
                StateHasChanged();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching issues: {ex.Message}");
            }
        }

        private async Task OpenIssue()
        {
            try
            {
                await IssueService.Open(UpdateDto);
                await GetAllIssue(ProjectId);
                StateHasChanged();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching issues: {ex.Message}");
            }
        }
        //private void OnMilestoneSelectChanged(ChangeEventArgs e)
        //{

        //    try
        //    {
        //        if (e.Value is string selectedValues)
        //        {
        //            UpdateDto.Milestone = UpdateDto.Milestone.Append(selectedValues).ToArray();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Error fetching issues: {ex.Message}");
        //    }
        //}
        private void OnLabelSelectChanged(ChangeEventArgs e)
        {
            try
            {
                if (e.Value is string selectedValues)
                {
                    UpdateDto.Labels = UpdateDto.Labels.Append(selectedValues).ToArray();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching issues: {ex.Message}");
            }
        }

        private void OnAssigneeSelectChanged(ChangeEventArgs e)
        {
            try
            {
                if (e.Value is string selectedValues)
                {
                    //UpdateDto.Assignees = UpdateDto.Assignees.Append(selectedValues).ToArray();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching issues: {ex.Message}");
            }
        }

        private async Task CreateIssue()
        {
            try
            {
                var newIssue = await IssueService.Create(AddDto);
                AddDto = new();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching issues: {ex.Message}");
            }
        }

        private async Task UpdateIssue()
        {
            try
            {
                var updatedIssue = await IssueService.Update(UpdateDto);
                UpdateDto = new();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching issues: {ex.Message}");
            }
        }

        private async Task SearchIssues()
        {
            await LoadIssues();
            await GetAllMilestone();
            await GetAllLabels();
            HasSearched = true;
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

        #region Common Services

        private async Task GetAllLabels()
        {
            try
            {
                Labels = [];
                Labels = await CommonService.GetLabelsByProject(ProjectId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching issues: {ex.Message}");
            }
        }
        private async Task GetAllMilestone()
        {
            try
            {
                Milestones = [];
                //---------------------------------\\
                //HARD CODE FOR MODERNIZATION GROUP\\
                //---------------------------------\\
                //---------------------------------\\

                var milestoneTitles = new List<string>();
                var res =  await CommonService.GetMilestonesByGroup(27);
                foreach (var item in res) {
                    milestoneTitles.Add(item.Title);
                }
                Milestones = milestoneTitles.ToArray();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching issues: {ex.Message}");
            }
        }
        #endregion

    }
}
