using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.JSInterop;

namespace Microsoft.Identity.Firebase.Models
{
    public class FirebaseSignOutSessionStateManager : SignOutSessionStateManager
    {
        public FirebaseSignOutSessionStateManager(IJSRuntime jsRuntime) : base(jsRuntime)
        {
        }
    }
}
