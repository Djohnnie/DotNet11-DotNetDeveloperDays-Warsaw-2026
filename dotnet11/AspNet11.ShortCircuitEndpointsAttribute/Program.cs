using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

// The new [ShortCircuit] attribute marks an endpoint to run immediately after routing, skipping
// the rest of the middleware pipeline (authentication, CORS, authorization, etc.).
// This is the attribute form of the existing ShortCircuit() endpoint convention, so it works on
// MVC controllers/actions as well as minimal API endpoints.
// It's useful for endpoints that don't need the rest of the pipeline, such as a health check or a
// robots.txt response - it avoids paying the cost of running that middleware.
// The endpoint still runs and produces its response; pass an optional status code, such as
// [ShortCircuit(404)], to set the response status instead.

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Custom middleware that would run for every request that reaches it - used here to prove, via
// the console output, that [ShortCircuit] skips it for the attributed endpoint.
app.Use(async (context, next) =>
{
    Console.WriteLine($"[middleware] ran for {context.Request.Path}");
    await next();
});

// Short-circuited: routing matches this endpoint and runs it directly, the logging middleware
// above never executes for this request.
app.MapGet("/health", [ShortCircuit] () => "Healthy");

// Not short-circuited: the logging middleware above runs first, exactly as before.
app.MapGet("/echo", () => "echo");

app.Run();
