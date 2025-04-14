using WebGitManager.Dto;

namespace WebGitManager.Pages
{
    public partial class Issue
    {
        private List<IssueDto> Issues { get; set; }
        private bool Loading { get; set; }
        private string ErrorMessage { get; set; }
        

        private async Task LoadIssues()
        {
            if (ProjectId <= 0)
            {
                ErrorMessage = "Please enter a valid project ID";
                return;
            }

            try
            {
                Loading = true;
                ErrorMessage = null;
                var response = (await GitManager.GetIssuesAsync(ProjectId)).ToList();
                Issues = response.Select(x => new Issue {
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to load issues: {ex.Message}";
                Logger.LogError(ex, "Error loading issues for project {ProjectId}", ProjectId);
            }
            finally
            {
                Loading = false;
            }
        }

        private void ClearError()
        {
            ErrorMessage = null;
        }

        private void SelectIssue(Issue issue)
        {
            EditingIssue = true;
            CurrentIssueId = issue.Id;
            IssueTitle = issue.Title;
            IssueDescription = issue.Description;

            // Parse labels if they exist
            if (issue.Labels != null && issue.Labels.Any())
            {
                IssueLabels = string.Join(",", issue.Labels);
            }
            else
            {
                IssueLabels = string.Empty;
            }

            // Parse assignee IDs if they exist
            if (issue.Assignees != null && issue.Assignees.Any())
            {
                AssigneeIdsInput = string.Join(",", issue.Assignees.Select(a => a.Id));
            }
            else
            {
                AssigneeIdsInput = string.Empty;
            }
        }

        private void CancelEdit()
        {
            EditingIssue = false;
            ClearForm();
        }

        private void ClearForm()
        {
            IssueTitle = string.Empty;
            IssueDescription = string.Empty;
            IssueLabels = string.Empty;
            AssigneeIdsInput = string.Empty;
            CurrentIssueId = 0;
        }

        private async Task SaveIssue()
        {
            if (ProjectId <= 0)
            {
                ErrorMessage = "Please enter a valid project ID";
                return;
            }

            if (string.IsNullOrWhiteSpace(IssueTitle))
            {
                ErrorMessage = "Issue title is required";
                return;
            }

            try
            {
                Loading = true;
                ErrorMessage = null;

                if (EditingIssue)
                {
                    // Parse assignee IDs
                    long[] assigneeIds = null;
                    if (!string.IsNullOrWhiteSpace(AssigneeIdsInput))
                    {
                        assigneeIds = AssigneeIdsInput.Split(',')
                            .Where(id => !string.IsNullOrWhiteSpace(id))
                            .Select(id => long.Parse(id.Trim()))
                            .ToArray();
                    }

                    await GitManager.UpdateIssueAsync(
                        ProjectId,
                        CurrentIssueId,
                        assigneeIds,
                        IssueTitle,
                        IssueDescription
                    );
                }
                else
                {
                    string[] labels = null;
                    if (!string.IsNullOrWhiteSpace(IssueLabels))
                    {
                        labels = IssueLabels.Split(',')
                            .Where(label => !string.IsNullOrWhiteSpace(label))
                            .Select(label => label.Trim())
                            .ToArray();
                    }

                    await GitManager.CreateIssueAsync(
                        ProjectId,
                        IssueTitle,
                        IssueDescription,
                        labels
                    );
                }

                // Reload issues and clear form
                await LoadIssues();
                ClearForm();
                EditingIssue = false;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to {(EditingIssue ? "update" : "create")} issue: {ex.Message}";
                Logger.LogError(ex, "Error {Action} issue for project {ProjectId}", EditingIssue ? "updating" : "creating", ProjectId);
            }
            finally
            {
                Loading = false;
            }
        }

        private async Task CloseIssue(int issueId)
        {
            if (ProjectId <= 0)
            {
                ErrorMessage = "Please enter a valid project ID";
                return;
            }

            try
            {
                Loading = true;
                ErrorMessage = null;

                await GitManager.CloseIssueAsync(ProjectId, issueId);
                await LoadIssues();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to close issue: {ex.Message}";
                Logger.LogError(ex, "Error closing issue {IssueId} for project {ProjectId}", issueId, ProjectId);
            }
            finally
            {
                Loading = false;
            }
        }
    }
}
