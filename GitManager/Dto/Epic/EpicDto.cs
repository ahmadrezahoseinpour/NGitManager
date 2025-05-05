using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace GitManager.Dto.Epic
{
    public class EpicDto
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("iid")]
        public long EpicIid { get; set; }

        [JsonPropertyName("group_id")]
        public long GroupId { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("labels")]
        public string[] Labels { get; set; }

        [JsonPropertyName("state")]
        public EpicStateDto State { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("updated_at")]
        public DateTime UpdatedAt { get; set; }

        [JsonPropertyName("web_url")]
        public string WebUrl { get; set; }
    }
}
