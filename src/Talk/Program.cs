using Talk.Components;
using Talk.Components.Demos;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MudBlazor.Services;
using BlazorDeck.Services;
using MvvmSample;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddMudServices();
builder.Services.AddScoped<DeckState>();

// MVVM sample services (view models + their dependencies) for the /demo/mvvm host page.
builder.Services.AddMvvmSample();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(Talk.Client._Imports).Assembly);

// Standalone render-mode demos, served OUTSIDE the deck's globally-interactive <Routes>.
// Each is its own static SSR document (via RazorComponentResult) that a slide embeds in an
// <iframe> — the iframe boundary keeps them isolated from the deck's interactive router.
app.MapGet("/demo/static-ssr",
    (int count = 0) => new RazorComponentResult<StaticSsrDemo>(
        new Dictionary<string, object?> { ["Count"] = count }));
app.MapPost("/demo/static-ssr",
    ([FromForm] int count) => new RazorComponentResult<StaticSsrDemo>(
        new Dictionary<string, object?> { ["Count"] = count + 1 }));
app.MapGet("/demo/render-modes",
    () => new RazorComponentResult<RenderModesDemo>());

// MVVM sample: a static host document with the sample rendered as an InteractiveServer island,
// so its button + busy-state actually run. The pattern's source lives in samples/mvvm/MvvmSample.
app.MapGet("/demo/mvvm",
    () => new RazorComponentResult<MvvmDemo>());

app.Run();
