using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace GitManager.Dto.Issue
{
    public class IssueEpicDto
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("iid")]
        public long EpicId { get; set; }

        [JsonPropertyName("group_id")]
        public long GroupId { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }
    }
}