using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
//using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
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


        public static string GetOpenIDConfigurationUrl(FirebaseProjectConfiguration config) =>
            $"https://securetoken.google.com/{config.projectId}";

        public const string SigningKeysUrl = "https://www.googleapis.com/robot/v1/metadata/x509/securetoken@system.gserviceaccount.com";


        /*
        public static async Task<List<X509SecurityKey>> GetIssuerSigningKeysAsync(HttpClient? httpClient = null)
        {
            httpClient ??= new HttpClient();
            var jsonResult = await httpClient.GetStreamAsync(SigningKeysUrl);
            var jsonNode = await JsonSerializer.DeserializeAsync<Dictionary<string,string>>(jsonResult);

            List<X509SecurityKey> x509IssuerSigningKeys = new List<X509SecurityKey>();

            //Extract X509SecurityKeys from JSON result
            foreach (var certificateString in jsonNode.Values)
            {
                var newKey = BuildSecurityKey(certificateString!);
                x509IssuerSigningKeys.Add(newKey);
            }
            return x509IssuerSigningKeys;
        }

        private static X509SecurityKey BuildSecurityKey(string certificate)
        {
            //Removing "-----BEGIN CERTIFICATE-----" and "-----END CERTIFICATE-----" lines
            var lines = certificate.Split('\n');
            var selectedLines = lines.Skip(1).Take(lines.Length - 3);
            var key = string.Join(Environment.NewLine, selectedLines);

            return new X509SecurityKey(new X509Certificate2(Convert.FromBase64String(key)));
        }
        */

        public static async Task<FirebaseOpenIdConfiguration> GetFirebaseOpenIdConfigurationAsync(string configurationUrl, HttpClient? httpClient = null)
        {
            httpClient ??= new System.Net.Http.HttpClient();
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            //var signingKeys = await GetIssuerSigningKeysAsync(httpClient);
            var response = (await httpClient.GetAsync(configurationUrl));
            var content = await response.Content.ReadAsStringAsync();
            var result = (await System.Text.Json.JsonSerializer.DeserializeAsync<FirebaseOpenIdConfiguration>(response.Content.ReadAsStream())!)!;
            return result;
        }

        public static async Task<FirebaseOpenIdConfiguration> GetFirebaseOpenIdConfigurationAsync(FirebaseProjectConfiguration configuration)
        {
            return await GetFirebaseOpenIdConfigurationAsync(GetOpenIDConfigurationUrl(configuration));
        }

        public static FirebaseOpenIdConfiguration GetFirebaseOpenIdConfiguration(FirebaseProjectConfiguration firebaseConfig)
        {
            var configurationData = Task.Run(async () => 
                await GetFirebaseOpenIdConfigurationAsync(firebaseConfig));
            configurationData.Wait();
            return configurationData.Result;
        }

        /*
        public static async Task Register(Firebase.Models.FirebaseOpenIdConfiguration firebaseOpenIdConfiguration)
        {
            List<X509SecurityKey> issuerSigningKeys = await GetIssuerSigningKeysAsync();

            app.UseJwtBearerAuthentication(new JwtBearerAuthenticationOptions
            {
                AuthenticationMode = AuthenticationMode.Active,
                AllowedAudiences = new[] { FirebaseValidAudience },
                Provider = new OAuthBearerAuthenticationProvider
                {
                    OnValidateIdentity = OnValidateIdentity
                },
                TokenValidationParameters = new TokenValidationParameters
                {
                    IssuerSigningKeys = issuerSigningKeys,
                    ValidAudience = FirebaseValidAudience,
                    ValidIssuer = FirebaseValidIssuer,
                    IssuerSigningKeyResolver = (arbitrarily, declaring, these, parameters) => issuerSigningKeys
                }

            });
        }
        */

        /*
        private static Task OnValidateIdentity(OAuthValidateIdentityContext context)
        {
            var calimsIdentity = context.Ticket.Identity;

            //Add custom claim if needed
            calimsIdentity.AddClaim(new Claim("ClaimType", "ClaimValue"));

            return Task.FromResult<int>(0);
        }
        */
    }
}