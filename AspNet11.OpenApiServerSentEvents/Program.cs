using System.Net.ServerSentEvents;
using System.Runtime.CompilerServices;

// Requires the .NET 11 Preview 7 SDK or later (this OpenAPI "itemSchema" refinement for SSE
// endpoints ships in the Microsoft.AspNetCore.OpenApi 11.0.0-preview.7 package, and that package
// needs the matching Preview 7 shared runtime - see the project-local global.json).
//
// Endpoints that return SseItem<T> (Server-Sent Events, introduced for TypedResults in .NET 10)
// are now described in the generated OpenAPI document with the OpenAPI 3.2 "itemSchema" shape for
// text/event-stream responses, instead of falling back to a plain "string" schema.
// The itemSchema describes a stream's per-event payload shape (here, a Todo), plus the standard
// SSE "event" / "id" string fields - so API consumers and tooling can see exactly what each event
// on the stream looks like.
//
// Notes on the API itself:
// - Return the stream through TypedResults.ServerSentEvents.
// - A handler that returns IAsyncEnumerable<SseItem<T>> directly (without TypedResults) is
//   serialized as JSON instead of SSE - use the dedicated SseItem<T> overload without eventType.
// - To use one event name for the whole stream, pass a plain IAsyncEnumerable<T> with eventType
//   instead (as in dotnet10/AspNet10.ServerSentEvents).

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();

var app = builder.Build();

app.MapOpenApi();

app.MapGet("/todos/stream", (CancellationToken ct) =>
    TypedResults.ServerSentEvents(GetTodosAsync(ct)))
   .WithName("StreamTodos");

app.Run();

static async IAsyncEnumerable<SseItem<Todo>> GetTodosAsync(
    [EnumeratorCancellation] CancellationToken ct = default)
{
    foreach (var todo in Todos.All)
    {
        yield return new SseItem<Todo>(todo) { EventId = todo.Id.ToString() };
        await Task.Delay(1000, ct);
    }
}

public record Todo(int Id, string Title, bool IsComplete);

public static class Todos
{
    public static readonly Todo[] All =
    [
        new Todo(1, "Prepare .NET 11 demo", false),
        new Todo(2, "Rehearse the talk", false),
        new Todo(3, "Ship the slides", true),
    ];
}
