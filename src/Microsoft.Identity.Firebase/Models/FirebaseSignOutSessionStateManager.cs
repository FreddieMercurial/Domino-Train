using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Identity.Firebase.Components;
using Microsoft.JSInterop;

namespace Microsoft.Identity.Firebase.Models
{
    public class FirebaseSignOutSessionStateManager : SignOutSessionStateManager
    {
        private readonly IJSRuntime _jsRuntime;
        public FirebaseSignOutSessionStateManager(IJSRuntime jsRuntime) : base(jsRuntime)
        {
            this._jsRuntime = jsRuntime;
        }

        public override ValueTask SetSignOutState()
        {
            this._jsRuntime!.InvokeVoidAsync("window.firebaseSignOut");
            return new ValueTask();
        }
    }
}
