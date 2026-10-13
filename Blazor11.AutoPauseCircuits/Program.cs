using Blazor11.AutoPauseCircuits.Components;
using Microsoft.AspNetCore.Components;

// .NET 11 Preview 7 adds Microsoft.AspNetCore.Components.Server.AutoPause, which automatically
// pauses a Blazor Server circuit when the browser tab is hidden - freeing the server resources
// (SignalR connection, circuit memory) a background tab would otherwise hold onto - and resumes
// it when the tab becomes visible again. This requires the .NET 11 Preview 7 SDK: the
// WithBrowserOptions endpoint hook this feature relies on didn't exist yet in Preview 6.
// See this project's global.json.

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
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
    .WithBrowserOptions(options =>
    {
        options.AddAutoPause(pause =>
        {
            pause.Enabled = true; // default
            pause.HiddenDelay = TimeSpan.FromSeconds(30); // default is 2 minutes
        });
    });

app.Run();
