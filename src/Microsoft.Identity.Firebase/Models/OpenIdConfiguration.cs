using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Microsoft.Identity.Firebase.Models
{
    public record OpenIdConfiguration
    {
        [JsonPropertyName("issuer")]
        public string Issuer { get; set; } = null!;

        [JsonPropertyName("response_types_supported")]
        public IList<string> ResponseTypesSupported { get; set; } = null!;

        [JsonPropertyName("scopes_supported")]
        public IList<string> ScopesSupported { get; set; } = null!;

        [JsonPropertyName("response_modes_supported")]
        public IList<string> ResponseModesSupported { get; set; } = null!;

        [JsonPropertyName("token_endpoint")]
        public string TokenEndpoint { get; set; } = null!;

        [JsonPropertyName("authorization_endpoint")]
        public string AuthorizationEndpoint { get; set; } = null!;

        [JsonPropertyName("registration_endpoint")]
        public string RegistrationEndpoint { get; set; } = null!;

        [JsonPropertyName("token_endpoint_auth_methods_supported")]
        public IList<OpenIdConfigurationTokenEndpointAuthMethod> TokenEndpointAuthMethodsSupported { get; set; } =
            new List<OpenIdConfigurationTokenEndpointAuthMethod>();
    }
}
