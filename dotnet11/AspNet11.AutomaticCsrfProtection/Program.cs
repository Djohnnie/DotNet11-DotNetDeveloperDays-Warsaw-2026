// Apps built with WebApplication.CreateBuilder now automatically reject unsafe cross-origin
// requests based on the browser's Sec-Fetch-Site and Origin headers.
// This lightweight cross-site request forgery (CSRF) protection is on with no configuration and
// applies across Minimal APIs, MVC, Razor Pages, and Blazor.
// Same-origin requests, user-initiated navigations, and non-browser clients (curl, HttpClient,
// Postman) are allowed, while a cross-origin browser request that tries to consume a form is
// rejected. It can be used in place of or alongside the existing token-based antiforgery system.
//
// - To opt a single endpoint out, call .DisableAntiforgery() on it (or use [IgnoreAntiforgeryToken]
//   in MVC).
// - To turn the feature off for the whole app, set the "DisableCsrfProtection" configuration key.
// - For full control over the trust decision, register a custom ICsrfProtection implementation.

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Protected by default: a cross-origin browser request posting here is rejected automatically,
// with no [ValidateAntiForgeryToken] or app.UseAntiforgery() needed.
app.MapPost("/comments", (CommentRequest request) => Results.Created("/comments/1", request));

// Opted out explicitly, e.g. because this endpoint is meant to be called cross-origin by design
// (a public webhook, or an API consumed by another origin that authenticates a different way).
app.MapPost("/webhooks/payment-provider", (WebhookPayload payload) => Results.Ok())
   .DisableAntiforgery();

app.Run();

public record CommentRequest(string Author, string Text);

public record WebhookPayload(string EventType, string Payload);
