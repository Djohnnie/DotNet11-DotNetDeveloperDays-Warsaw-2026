using Blazor11.SupplyParameterFromTempData.Components;

// .NET 11 adds [SupplyParameterFromTempData], which binds a component parameter to TempData -
// the classic post-redirect-get "one-time success message" pattern, now natively supported for
// server-rendered Blazor components. Unlike [SupplyParameterFromQuery]/[SupplyParameterFromForm],
// this parameter is read/write: setting it during request handling (e.g. a static SSR form post)
// writes it through to TempData, and it's read back automatically on the next request - see
// Components/Pages/Home.razor for the demo.

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
    .AddInteractiveServerRenderMode();

app.Run();
