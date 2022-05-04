using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace Microsoft.Identity.Firebase.Models
{
    [Serializable]
    public class FirebaseUser : RemoteUserAccount
    {
        [JsonPropertyName("uid")]
        public string uid { get; set; }
        [JsonPropertyName("email")]
        public string email { get; set; }
        [JsonPropertyName("emailVerified")]
        public bool emailVerified { get; set; }
        [JsonPropertyName("isAnonymous")]
        public bool isAnonymous { get; set; }
        [JsonPropertyName("providerData")]
        public IEnumerable<FirebaseProviderData> providerData { get; set; }
        [JsonPropertyName("stsTokenManager")]
        public StsTokenManager stsTokenManager { get; set; }
        [JsonPropertyName("createdAt")]
        public string createdAt { get; set; }
        [JsonPropertyName("lastLoginAt")]
        public string lastLoginAt { get; set; }
        [JsonPropertyName("apiKey")]
        public string apiKey { get; set; }
        [JsonPropertyName("appName")]
        public string appName { get; set; }
    }
}
