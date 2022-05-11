using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace Microsoft.Identity.Firebase.Models
{
    [Serializable]
    public class FirebaseProviderData : RemoteUserAccount
    {
        [JsonPropertyName("providerId")]
        public string ProviderId { get; set; }
        [JsonPropertyName("uid")]
        public string FirebaseUid { get; set; }
        [JsonPropertyName("displayName")]
        public string? DisplayName { get; set; }
        [JsonPropertyName("email")]
        public string Email { get; set; }
        [JsonPropertyName("phoneNumber")]
        public string? PhoneNumber { get; set; }
        [JsonPropertyName("photoURL")]
        public string? PhotoUrl { get; set; }
    }
}
