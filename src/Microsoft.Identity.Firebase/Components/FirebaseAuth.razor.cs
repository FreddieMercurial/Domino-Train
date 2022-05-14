using Microsoft.AspNetCore.Components;
using Microsoft.Identity.Firebase.Models;
using Microsoft.JSInterop;
using System.ComponentModel;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Components.Authorization;

namespace Microsoft.Identity.Firebase.Components
{
    public partial class FirebaseAuth : ComponentBase
    {
        [Inject] public static FirebaseProjectConfiguration? FirebaseConfiguration { get; private set; }

        public static void SetFirebaseConfiguration(FirebaseProjectConfiguration configuration)
        {
            if (FirebaseConfiguration is null)
            {
                FirebaseConfiguration = configuration;
            }
        }


        [Inject] private static IJSRuntime StaticJsInterop { get; set; }

        [Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; set; }

        [Inject] private static NavigationManager StaticNavigationManager { get; set; }

        [Bindable(true)]
        public static bool IsAuthenticated { get; private set; }

        [Bindable(true)]
        public static FirebaseUser? CurrentUser { get; private set; }

        private static bool Initialized { get; set; } = false;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (!Initialized && _jsRuntime is not null)
            {
                await _jsRuntime.InvokeVoidAsync("window.firebaseInitialize", DotNetObjectReference.Create(this));
                Initialized = true;
            }
        }

        [JSInvokable]
        public async Task OnAuthStateChanged(FirebaseUser? user)
        {
            if (user == null)
            {
                IsAuthenticated = false;
                CurrentUser = null;
            }
            else
            {
                CurrentUser = user;
            }
            StateProvider.InvokeNotifyAuthenticationStateChanged();
        }

        private static async Task<FirebaseUser?> CreateEmailUser(string email, string password)
        {
            var userData = await StaticJsInterop!.InvokeAsync<string?>("window.firebaseCreateUser", email, password);
            if (string.IsNullOrEmpty(userData))
                return null;
            var userDataBytes = userData.ToCharArray().Select(c => (byte)c).ToArray();
            var userDataStream = new MemoryStream(userDataBytes);
            var jsonSerializerOptions = new JsonSerializerOptions
            {
                AllowTrailingCommas = true,
                UnknownTypeHandling = JsonUnknownTypeHandling.JsonElement,
                DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.Never,
                IgnoreReadOnlyFields = false,
                IgnoreReadOnlyProperties = false
            };
            var userObject = await JsonSerializer.DeserializeAsync<FirebaseUser>(
                utf8Json: userDataStream,
                options: jsonSerializerOptions);
            StateProvider.InvokeNotifyAuthenticationStateChanged();
            return userObject;
        }

        public async Task SignOut()
        {
            await _jsRuntime!.InvokeVoidAsync("window.firebaseSignOut", DotNetObjectReference.Create(this));
        }
    }
}
