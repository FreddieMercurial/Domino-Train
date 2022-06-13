using DominoTrain.WebCli;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using static HACC.Extensions.HaccExtensions;
using static Microsoft.Identity.Firebase.Extensions.FirebaseAuthExtensions;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
await builder.UseFirebaseAuthenticationAsync();
builder.UseHacc();
await builder.Build().RunAsync();
