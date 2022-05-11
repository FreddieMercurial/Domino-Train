using System.Text.Json.Serialization;

namespace Microsoft.Identity.Firebase.Models
{
    [Serializable]
    public class StsTokenManager
    {
        [JsonPropertyName("refreshToken")]
        public string RefreshToken { get; set; }
        [JsonPropertyName("accessToken")]
        public string AccessToken { get; set; }
        [JsonPropertyName("expirationTime")]
        public ulong ExpirationTime { get; set; }
    }
}
