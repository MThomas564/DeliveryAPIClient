using DeliveryAPIClient.Extensions;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<DeliveryAPIClient.Examples.App>("#app");

// Configuration is loaded automatically from wwwroot/appsettings.json
// and wwwroot/appsettings.local.json (if present — see wwwroot/appsettings.local.json).
builder.Services.AddUmbracoDeliveryApiClient(options =>
{
    builder.Configuration.GetSection("UmbracoDeliveryApi").Bind(options);
});

await builder.Build().RunAsync();
