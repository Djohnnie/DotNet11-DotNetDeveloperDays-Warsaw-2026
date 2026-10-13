using Blazor11.EnvironmentView.Components;

// .NET 11 adds the EnvironmentView component, which conditionally renders its content based on
// the current hosting environment (Development / Staging / Production / a custom environment
// name). It replaces hand-rolled "@if (Environment.IsDevelopment())" checks with a declarative
// markup block, similar in spirit to ASP.NET Core MVC's old <environment> tag helper.
// See Components/Pages/Home.razor for the demo.

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
