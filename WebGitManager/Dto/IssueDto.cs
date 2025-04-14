namespace WebGitManager.Dto
{
    public class IssueDto
    {

        private int ProjectId { get; set; }
        private int IssueId { get; set; }
        private string IssueTitle { get; set; }
        private string IssueDescription { get; set; }
        private string IssueLabels { get; set; }
        private string AssigneeIdsInput { get; set; }
        private bool EditingIssue { get; set; }
        private int CurrentIssueId { get; set; }
    }
}
