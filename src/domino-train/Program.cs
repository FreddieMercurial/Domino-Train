using System.Security.Principal;
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

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddOidcAuthentication(options =>
{
    builder.Configuration.Bind("Local", options.ProviderOptions);
});

//builder.Services.AddOptions();
//builder.Services.AddAuthorizationCore();

// without this component, the authentication goes through basic google auth rather than firebase specifically
// although the OIDC realm is correct, it is not reading the correct user information
builder.Services.AddScoped<StateProvider>();
builder.Services.AddScoped<IdentityUser, FirebaseUser>();
builder.Services.AddScoped<AuthenticationStateProvider>(s => s.GetRequiredService<StateProvider>());
builder.Services.AddScoped<SignOutSessionStateManager, FirebaseSignOutSessionStateManager>();
builder.Services.AddSingleton<FirebaseConfiguration>(implementationInstance: new FirebaseConfiguration(builder.Configuration));
builder.Services.AddSingleton<FirebaseAuth>();


await builder.Build().RunAsync();
