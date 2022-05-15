using DominoTrain;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Identity.Firebase.Components;
using Microsoft.Identity.Firebase.Models;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var httpClient = new HttpClient {
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
};
var openIdConfiguration = await FirebaseOpenIdConfiguration.GetFirebaseOpenIdConfigurationAsync(
        configurationUrl: string.Join(httpClient.BaseAddress.ToString(), "/openIdConfiguration.json"),
        httpClient: httpClient);
var firebaseConfiguration = new FirebaseProjectConfiguration(builder.Configuration, openIdConfiguration);
var firebaseAuth = new FirebaseAuth();
FirebaseAuth.SetFirebaseConfiguration(firebaseConfiguration);

builder.Services.AddOidcAuthentication(options =>
{
    builder.Configuration.Bind("Local", options.ProviderOptions);
});
builder.Services.AddScoped(sp => httpClient);
builder.Services.AddSingleton<FirebaseOpenIdConfiguration>(implementationInstance: openIdConfiguration);
builder.Services.AddSingleton<FirebaseProjectConfiguration>(implementationInstance: firebaseConfiguration);
builder.Services.AddSingleton<FirebaseAuth>(implementationInstance: firebaseAuth);
builder.Services.AddSingleton<StateProvider>();
builder.Services.AddScoped<IdentityUser, FirebaseUser>();
builder.Services.AddSingleton<AuthenticationStateProvider>(s => s.GetRequiredService<StateProvider>());
builder.Services.AddSingleton<SignOutSessionStateManager, FirebaseSignOutSessionStateManager>();

await builder.Build().RunAsync();
