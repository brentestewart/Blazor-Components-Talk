using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using BlazorDeck.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddScoped<DeckState>();

await builder.Build().RunAsync();
