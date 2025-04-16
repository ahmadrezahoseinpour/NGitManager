using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace GitManager.Dto.Issue
{
    public class AssigneeDto
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("state")]
        public string State { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("avatar_url")]
        public string AvatarURL { get; set; }
    }
}
