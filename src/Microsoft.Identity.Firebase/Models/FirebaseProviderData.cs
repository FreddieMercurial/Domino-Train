using System.Text.Json.Serialization;

namespace Microsoft.Identity.Firebase.Models
{
    [Serializable]
    public struct FirebaseProviderData
    {
        [JsonPropertyName("providerId")]
        public string providerId { get; set; }
        [JsonPropertyName("uid")]
        public string uid { get; set; }
        [JsonPropertyName("displayName")]
        public string? displayName { get; set; }
        [JsonPropertyName("email")]
        public string email { get; set; }
        [JsonPropertyName("phoneNumber")]
        public string? phoneNumber { get; set; }
        [JsonPropertyName("photoURL")]
        public string? photoUrl { get; set; }
    }
}
