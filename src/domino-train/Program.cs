using DominoTrain;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using static Microsoft.Identity.Firebase.Extensions.FirebaseAuthExtensions;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

await builder.UseFirebaseAuthenticationAsync();

await builder.Build().RunAsync();
