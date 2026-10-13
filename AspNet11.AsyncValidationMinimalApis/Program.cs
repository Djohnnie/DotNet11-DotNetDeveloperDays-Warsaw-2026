using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

// Minimal API validation now supports asynchronous validators end-to-end.
// Two new base-library APIs make this possible:
// - AsyncValidationAttribute: derive from it and implement IsValidAsync to run a validation
//   rule that needs to await something (a database call, a remote API) without blocking a thread.
// - IAsyncValidatableObject: implement it (it extends IValidatableObject) to run validation that
//   spans several properties or the whole object, returning results as an IAsyncEnumerable<ValidationResult>.
// Microsoft.Extensions.Validation runs both automatically once builder.Services.AddValidation() is
// registered - the framework validates the request before the endpoint runs, the same as with
// synchronous DataAnnotations validation.
// Validators run concurrently where possible: asynchronous attributes on the same member start
// together, and collection items validate in parallel.

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddValidation();
builder.Services.AddSingleton<IUserService, InMemoryUserService>();
builder.Services.AddSingleton<IRoomService, InMemoryRoomService>();

var app = builder.Build();

app.MapPost("/users", (RegisterUserRequest request) 
    => Results.Created($"/users/{request.Email}", request));




app.MapPost("/reservations", (ReservationRequest request) => Results.Ok(request));

app.Run();

// A custom async validation attribute: query a "database" without blocking a thread.
public sealed class UniqueEmailAttribute : AsyncValidationAttribute
{
    // This attribute validates asynchronously only, so the synchronous fallback must throw.
    protected override ValidationResult? IsValid(object? value, ValidationContext context) =>
        throw new InvalidOperationException("Validate this attribute with IsValidAsync.");

    protected override async Task<ValidationResult?> IsValidAsync(
        object? value, ValidationContext context, CancellationToken cancellationToken)
    {
        var users = context.GetRequiredService<IUserService>();
        if (value is string email && await users.EmailExistsAsync(email, cancellationToken))
        {
            return new ValidationResult("That email is already registered.");
        }

        return ValidationResult.Success;
    }
}

public record RegisterUserRequest([property: Required, UniqueEmail] string Email);

// Object-level async validation: spans several properties, so it implements IAsyncValidatableObject.
public class ReservationRequest : IAsyncValidatableObject
{
    [Required]
    public string Email { get; set; } = "";

    public DateOnly Date { get; set; }

    // This type validates asynchronously only, so the synchronous fallback must throw.
    public IEnumerable<ValidationResult> Validate(ValidationContext context) =>
        throw new InvalidOperationException("Validate this type with ValidateAsync.");

    public async IAsyncEnumerable<ValidationResult> ValidateAsync(
        ValidationContext context,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var rooms = context.GetRequiredService<IRoomService>();
        if (!await rooms.HasAvailabilityAsync(Date, cancellationToken))
        {
            yield return new ValidationResult("No rooms are available on that date.", [nameof(Date)]);
        }
    }
}

public interface IUserService
{
    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken);
}

public sealed class InMemoryUserService : IUserService
{
    private static readonly HashSet<string> TakenEmails = ["taken@example.com"];

    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken)
    {
        await Task.Delay(50, cancellationToken); // simulate a database round-trip
        return TakenEmails.Contains(email);
    }
}

public interface IRoomService
{
    Task<bool> HasAvailabilityAsync(DateOnly date, CancellationToken cancellationToken);
}

public sealed class InMemoryRoomService : IRoomService
{
    private static readonly DateOnly FullyBookedDate = new(2026, 12, 24);

    public async Task<bool> HasAvailabilityAsync(DateOnly date, CancellationToken cancellationToken)
    {
        await Task.Delay(50, cancellationToken); // simulate a database round-trip
        return date != FullyBookedDate;
    }
}
