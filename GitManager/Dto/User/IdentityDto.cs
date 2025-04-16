using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace GitManager.Dto.User
{
    public class IdentityDto
    {
        [JsonPropertyName("provider")]
        public string Provider { get; set; }

        [JsonPropertyName("extern_uid")]
        public string ExternUid { get; set; }

        [JsonPropertyName("saml_provider_id")]
        public long? SamlProviderId { get; set; }
    }
}
