using Microsoft.AspNetCore.Identity;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace Microsoft.Identity.Firebase.Models
{
    /// <summary>
    /// The Firebase implementation of <see cref="IdentityUser"/> which uses a string as a primary key.
    /// </summary>
    [Serializable]
    public class FirebaseRemoteUserAccount : RemoteUserAccount
    {
        [JsonPropertyName("uid")] public string FirebaseUid { get; set; }

        /// <summary>
        /// The user's email address.
        /// Must be capitalized due to being inherited from <see cref="IdentityUser.Email"/>.
        /// </summary>
        [JsonPropertyName("email")] public string Email { get; set; }

        [JsonPropertyName("emailVerified")] public bool EmailVerified { get; set; }

        [JsonPropertyName("isAnonymous")] public bool IsAnonymous { get; set; }

        [JsonPropertyName("providerData")] public IEnumerable<FirebaseProviderData> ProviderData { get; set; }

        [JsonPropertyName("stsTokenManager")] public StsTokenManager StsTokenManager { get; set; }

        [JsonPropertyName("createdAt")] public string CreatedAt { get; set; }

        [JsonPropertyName("lastLoginAt")] public string LastLoginAt { get; set; }

        /// <summary>
        /// https://firebase.google.com/docs/projects/api-keys?msclkid=50c2da1bd15411ec864a2051a4985260
        /// API keys for Firebase are different from typical API keys
        /// Unlike how API keys are typically used, API keys for Firebase services are not used to control access to backend resources; that can only be done with Firebase Security Rules(to control which users can access resources) and App Check(to control which apps can access resources).
        /// Usually, you need to fastidiously guard API keys(for example, by using a vault service or setting the keys as environment variables); however, API keys for Firebase services are ok to include in code or checked-in config files.
        /// Although API keys for Firebase services are safe to include in code, there are a few specific cases when you should enforce limits for your API key; for example, if you're using Firebase ML, Firebase Authentication with the email/password sign-in method, or a billable Google Cloud API. Learn more about these cases later on this page.
        /// </summary>
        [JsonPropertyName("apiKey")] public string ApiKey { get; set; }
        [JsonPropertyName("appName")] public string AppName { get; set; }

        /// <summary>
        /// Initializes a new instance of <see cref="FirebaseUser"/>.
        /// </summary>
        /// <remarks>
        /// The Id property is initialized to form a new GUID string value.
        /// </remarks>
        public FirebaseRemoteUserAccount(FirebaseUser? user)
        {
            if (user is null)
            {
                return;
            }
            FirebaseUid = user.FirebaseUid;
            Email = user.Email;
            EmailVerified = user.EmailVerified;
            IsAnonymous = user.IsAnonymous;
            ProviderData = user.ProviderData;
            StsTokenManager = user.StsTokenManager;
            CreatedAt = user.CreatedAt;
            LastLoginAt = user.LastLoginAt;
        }
    }
}
