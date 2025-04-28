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
        [JsonIgnore]
        public long ProjectId { get; set; }

        [JsonPropertyName("issue_id")]
        public long IssueId { get; set; }

        [Required]
        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("assignee_id")]
        public long? AssigneeId { get; set; }

        [JsonPropertyName("assignee_ids")]
        public long[] AssigneeIds { get; set; }

        [JsonPropertyName("milestone_id")]
        public long? MileStoneId { get; set; }

        [JsonPropertyName("labels")]
        public string Labels { get; set; }

        [JsonPropertyName("confidential")]
        public bool Confidential { get; set; }

        [JsonPropertyName("due_date")]
        public DateTime? DueDate { get; set; }

        [JsonPropertyName("epic_id")]
        public long? EpicId { get; set; }

        [JsonPropertyName("weight")]
        public int? Weight { get; set; }

        [JsonPropertyName("state_event")]
        public string State { get; set; }

        [JsonPropertyName("discussion_locked")]
        public bool? DiscussionLocked { get; set; }

        [JsonPropertyName("author_id")]
        public long AuthorId { get; set; }

    }
}
