using NGitLab.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using GitManager.Dto.User;
using System.ComponentModel.DataAnnotations;

namespace GitManager.Dto.Issue
{
    public class IssueDto
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("iid")]
        public long IssueId { get; set; }

        [JsonPropertyName("project_id")]
        [Required(ErrorMessage = "شناسه پروژه اجباری است")]
        [Range(1, int.MaxValue, ErrorMessage ="شناسه پروژه را به درستی انتخاب کنید")]
        public long ProjectId { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("labels")]
        public string[] Labels { get; set; }

        [JsonPropertyName("milestone")]
        public MilestoneDto Milestone { get; set; }

        [JsonPropertyName("assignee")]
        public AssigneeDto Assignee { get; set; }

        [JsonPropertyName("assignees")]
        public AssigneeDto[] Assignees { get; set; }

        [JsonPropertyName("author")]
        public AuthorDto Author { get; set; }

        [JsonPropertyName("state")]
        public string State { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("updated_at")]
        public DateTime UpdatedAt { get; set; }

        [JsonPropertyName("closed_at")]
        public DateTime ClosedAt { get; set; }

        [JsonPropertyName("closed_by")]
        public UserDto ClosedBy { get; set; }

        [JsonPropertyName("due_date")]
        public DateTime? DueDate { get; set; }

        [JsonPropertyName("web_url")]
        public string WebUrl { get; set; }

        [JsonPropertyName("merge_requests_count")]
        public int MergeRequestsCount { get; set; }

        [JsonPropertyName("epic")]
        public IssueEpicDto Epic { get; set; }

        [JsonPropertyName("confidential")]
        public bool Confidential { get; set; }

        [JsonPropertyName("weight")]
        public int? Weight { get; set; } = 0;

        [JsonPropertyName("issue_type")]
        public string IssueType { get; set; }

        [JsonPropertyName("moved_to_id")]
        public long? MovedToId { get; set; }

        [JsonPropertyName("references")]
        public ReferencesDto References { get; set; }

        [JsonPropertyName("user_notes_count")]
        public int UserNotesCount { get; set; }

        [JsonPropertyName("discussion_locked")]
        public bool DiscussionLocked { get; set; }
    }
}
