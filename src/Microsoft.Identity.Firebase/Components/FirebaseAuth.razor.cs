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

        private static string? _currentFirebaseGuid = null;

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

        public static FirebaseAuth? Instance { get; private set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (!Initialized && _jsRuntime is not null)
            {
                Instance = this;
                await _jsRuntime.InvokeVoidAsync("window.firebaseInitialize", DotNetObjectReference.Create(this));
                Initialized = true;
            }
        }

        [JSInvokable]
        public async Task OnAuthStateChanged(FirebaseUser? user)
        {
            bool changed = CurrentUser is null && user is not null || CurrentUser is not null && user is null;
            bool changedUid = CurrentUser?.FirebaseUid != user?.FirebaseUid || _currentFirebaseGuid != user?.FirebaseUid;
            if (user == null)
            {
                IsAuthenticated = false;
                CurrentUser = null;
                _currentFirebaseGuid = null;
            }
            else
            {
                _currentFirebaseGuid = user.FirebaseUid;
                CurrentUser = user;
            }
            if (changed || changedUid) { 
               StateProvider.InvokeNotifyAuthenticationStateChanged();
            }
        }

        public static async Task<FirebaseUser> FirebaseUserFromJsonData(string userData)
        {
            if (string.IsNullOrEmpty(userData))
                throw new ArgumentNullException(nameof(userData));
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
            if (userObject is null)
                throw new InvalidOperationException("Unable to deserialize user data.");
            CurrentUser = userObject;
            StateProvider.InvokeNotifyAuthenticationStateChanged();
            return userObject;
        }
        
        public static async Task<FirebaseUser?> SignInEmailUser(string email, string password, IJSRuntime? jsRuntime = null)
        {
            if (jsRuntime is not null)
            {
                StaticJsInterop = jsRuntime;
            }
            var userData = await StaticJsInterop!.InvokeAsync<string?>("window.firebaseLoginUser", email, password);
            if (string.IsNullOrEmpty(userData))
                return null;
            return await FirebaseUserFromJsonData(userData);
        }

        public static async Task<FirebaseUser?> CreateEmailUser(string email, string password, IJSRuntime? jsRuntime = null)
        {
            if (jsRuntime is not null)
            {
                StaticJsInterop = jsRuntime;
            }
            var userData = await StaticJsInterop!.InvokeAsync<string?>("window.firebaseCreateUser", email, password);
            if (string.IsNullOrEmpty(userData))
                return null;
            return await FirebaseUserFromJsonData(userData);
        }

        public static async Task<bool> UpdateEmailUserData(FirebaseUser user, IJSRuntime? jSRuntime = null)
        {
            if (!CurrentUser!.FirebaseUid.Equals(user.FirebaseUid))
            {
                return false;
            }

            if (jSRuntime is not null)
            {
                StaticJsInterop = jSRuntime;
            }

            return await StaticJsInterop!.InvokeAsync<bool>("window.firebaseUpdateProfile", user.FirstProvider);
        }

        public async Task SignOutAsync()
        {
            await _jsRuntime!.InvokeVoidAsync("window.firebaseSignOut", DotNetObjectReference.Create(this));
        }
    }
}
