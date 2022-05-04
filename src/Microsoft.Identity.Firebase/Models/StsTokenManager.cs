using System.Text.Json.Serialization;

namespace Microsoft.Identity.Firebase.Models
{
    [Serializable]
    public class StsTokenManager
    {
        [JsonPropertyName("refreshToken")]
        public string refreshToken { get; set; }
        [JsonPropertyName("accessToken")]
        public string accessToken { get; set; }
        [JsonPropertyName("expirationTime")]
        public ulong expirationTime { get; set; }
    }
}
