using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.Identity.Firebase.Components;

namespace Microsoft.Identity.Firebase.Models
{
    public class StateProvider : AuthenticationStateProvider
    {
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
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var identity = new ClaimsIdentity();
            if (!FirebaseAuth.Authenticated)
            {
                var authState = new AuthenticationState(new ClaimsPrincipal(identity)); 
                return authState;
            }

            var user = FirebaseAuth.User!;
            var claims = ParseClaimsFromJwt(user.stsTokenManager.accessToken).ToList();
            identity = new ClaimsIdentity(claims, "firebase");
            return await Task.FromResult(new AuthenticationState(new ClaimsPrincipal(identity)));
        }

        public void ManageUser()
        {
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }
    }
}
