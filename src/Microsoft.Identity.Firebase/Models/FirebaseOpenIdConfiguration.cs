using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Microsoft.Identity.Firebase.Models
{
    [Serializable]
    public record FirebaseOpenIdConfiguration : OpenIdConfiguration
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        [JsonPropertyName("jwks_uri")]
        public string JwksUri { get; set; }

        [JsonPropertyName("subject_types_supported")]
        public string[] SubjectTypesSupported { get; set; }

        [JsonPropertyName("id_token_signing_alg_values_supported")]
        public string[] IdTokenSigningAalgValuesSupported { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.


        public static string GetOpenIDConfigurationUrl(FirebaseConfiguration config) =>
            $"https://securetoken.google.com/{config.projectId}";        
        
        public static async Task<FirebaseOpenIdConfiguration> GetFirebaseOpenIdConfigurationAsync(string configurationUrl, HttpClient? httpClient = null)
        {
            httpClient ??= new System.Net.Http.HttpClient();
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            var response = (await httpClient.GetAsync(configurationUrl));
            var content = await response.Content.ReadAsStringAsync();
            return (await System.Text.Json.JsonSerializer.DeserializeAsync<FirebaseOpenIdConfiguration>(response.Content.ReadAsStream())!)!;
        }

        public static async Task<FirebaseOpenIdConfiguration> GetFirebaseOpenIdConfigurationAsync(FirebaseConfiguration configuration)
        {
            return await GetFirebaseOpenIdConfigurationAsync(GetOpenIDConfigurationUrl(configuration));
        }

        public static FirebaseOpenIdConfiguration GetFirebaseOpenIdConfiguration(FirebaseConfiguration firebaseConfig)
        {
            var configurationData = Task.Run(async () => 
                await GetFirebaseOpenIdConfigurationAsync(firebaseConfig));
            configurationData.Wait();
            return configurationData.Result;
        }
    }
}