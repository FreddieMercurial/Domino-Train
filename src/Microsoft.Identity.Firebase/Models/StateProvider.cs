using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Identity.Firebase.Components;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Authorization;

namespace Microsoft.Identity.Firebase.Models
{
    public class StateProvider : AuthenticationStateProvider
    {
        private static StateProvider? _instance;
        public StateProvider()
        {
            if (_instance is not null)
                throw new InvalidOperationException("StateProvider is already initialized");
            _instance = this;
        }

        public static StateProvider Instance => _instance ?? throw new InvalidOperationException("StateProvider is not initialized");

        public static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var claims = new List<Claim>();
            var payload = jwt.Split('.')[1];

            var jsonBytes = ParseBase64WithoutPadding(payload);

            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);
            claims.AddRange(keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString()!)));
            return claims;
        }
        private static byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            return Convert.FromBase64String(base64);
        }

        public static async Task<AuthenticationState> GetAuthenticationStateAsyncStatic()
        {
            return await Task.FromResult(AuthenticationStateFromUser(FirebaseAuth.CurrentUser));
        }

        public static AuthenticationState AuthenticationStateFromUser(FirebaseUser? user)
        {
            var identity = new ClaimsIdentity();
            if (user is null)
            {
                var authState = new AuthenticationState(new ClaimsPrincipal(identity));
                return authState;
            }

            var claims = ParseClaimsFromJwt(user.StsTokenManager.AccessToken).ToList();
            identity = new ClaimsIdentity(claims, user.ProviderData.First().ProviderId);
            return new AuthenticationState(new ClaimsPrincipal(identity));
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            return await GetAuthenticationStateAsyncStatic();
        }

        public static void InvokeNotifyAuthenticationStateChanged()
        {
            Instance.ManageUser();
        }

        public void ManageUser()
        {
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }
    }
}
