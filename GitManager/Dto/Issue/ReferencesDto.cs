using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace GitManager.Dto.Issue
{
    public class ReferencesDto
    {
        [JsonPropertyName("short")]
        public string Short { get; set; }

        [JsonPropertyName("relative")]
        public string Relative { get; set; }

        [JsonPropertyName("full")]
        public string Full { get; set; }
    }
}
