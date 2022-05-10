using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Components;
using Microsoft.Identity.Firebase.Models;
using Microsoft.JSInterop;
using Microsoft.JSInterop.Implementation;

namespace Microsoft.Identity.Firebase.Components
{
    public partial class FirebaseAuth : ComponentBase
    {
        [Inject] private static IJSRuntime jsRuntime { get; set; } = null!;

        public static bool IsAuthenticated { get; private set; }

        public static FirebaseUser? CurrentUser { get; private set; }

        protected async override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);
            if (firstRender)
            {
                var dotNetHelper = DotNetObjectReference.Create(this);
                await jsRuntime!.InvokeVoidAsync("window.firebaseInitialize", dotNetHelper);
            }
        }

        [JSInvokable]
        public async Task OnAuthStateChanged(FirebaseUser? user)
        {
            if (user == null)
            {
                IsAuthenticated = false;
            }
            else
            {
                CurrentUser = user;
            }
        }

        private static async Task<FirebaseUser?> CreateUser(string email, string password)
        {
            var userData = await jsRuntime!.InvokeAsync<string?>("window.firebaseCreateUser", email, password);
            if (string.IsNullOrEmpty(userData))
                return null;
            var userDataBytes = userData.ToCharArray().Select(c => (byte) c).ToArray();
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
            return userObject;
        }

        public static string? GetFirebaseApiKey()
        {
            // if available locally, get api key from environment
            var environmentKey = Environment.GetEnvironmentVariable("FIREBASE_API_KEY");
            return !string.IsNullOrEmpty(environmentKey) ? environmentKey : null;
        }

        public static void SignOut()
        {
            CurrentUser = null;
        }
    }
}
