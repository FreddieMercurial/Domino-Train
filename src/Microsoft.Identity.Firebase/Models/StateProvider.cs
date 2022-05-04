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
using Microsoft.Identity.Firebase.Components;

namespace Microsoft.Identity.Firebase.Models
{
    public class StateProvider : AuthenticationStateProvider
    {
        public async override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var identity = new ClaimsIdentity();
            if (FirebaseAuth.Authenticated)
            {
                var user = FirebaseAuth.User!;

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.uid),
                        new Claim(ClaimTypes.Email, user.email),
                        new Claim(ClaimTypes.Role, "Basic User")
                    };
                    identity = new ClaimsIdentity(claims, "authentication");
                    return await Task.FromResult(new AuthenticationState(new ClaimsPrincipal(identity)));
            }
            AuthenticationState authState = new AuthenticationState(new ClaimsPrincipal(identity));
            return authState;
        }

        public void ManageUser()
        {
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }
    }
}
