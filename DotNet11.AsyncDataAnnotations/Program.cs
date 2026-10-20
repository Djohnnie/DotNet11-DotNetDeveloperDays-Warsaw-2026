// .NET 11 Libraries - Validate data asynchronously.
// Introduced: Preview 6.
//
// System.ComponentModel.DataAnnotations now supports
// asynchronous validation, so a rule that
// needs I/O (a database lookup, a remote API call) no longer
// has to block a thread. There are
// three new ways to express an async rule:
//   * derive from AsyncValidationAttribute and override
//     IsValidAsync,
//   * implement IAsyncValidatableObject on the model,
//   * call Validator.ValidateObjectAsync /
//     TryValidateObjectAsync / ValidatePropertyAsync /
//     ValidateValueAsync.
//
// Source:
// https://github.com/dotnet/core/blob/main/release-notes/11.0/preview/preview6/libraries.md#asynchronous-validation-with-dataannotations

using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection()
    .AddSingleton<IVatRegistry, FakeVatRegistry>()
    .BuildServiceProvider();

await ValidateAsync(new Invoice { VatNumber = "BE0123456789", Amount = 199.95m }, services);
await ValidateAsync(new Invoice { VatNumber = "XX0000000000", Amount = 199.95m }, services);
await ValidateAsync(new Invoice { VatNumber = "BE0123456789", Amount = -1m }, services);

Console.WriteLine("== TryValidateObjectAsync collects every failure ==");
var context = new ValidationContext(new Invoice { VatNumber = "XX0000000000", Amount = -5m }, services, items: null);
var results = new List<ValidationResult>();
bool valid = await Validator.TryValidateObjectAsync(context.ObjectInstance, context, results, validateAllProperties: true);
Console.WriteLine($"valid: {valid}");
foreach (ValidationResult result in results)
{
    Console.WriteLine($"  - {result.ErrorMessage}");
}

static async Task ValidateAsync(Invoice invoice, IServiceProvider services)
{
    Console.WriteLine($"== Validating VAT '{invoice.VatNumber}', amount {invoice.Amount} ==");
    var context = new ValidationContext(invoice, services, items: null);

    try
    {
        await Validator.ValidateObjectAsync(invoice, context, validateAllProperties: true);
        Console.WriteLine("  valid");
    }
    catch (ValidationException ex)
    {
        Console.WriteLine($"  invalid: {ex.Message}");
    }

    Console.WriteLine();
}

// 1. An attribute that validates asynchronously.
internal sealed class ValidVatNumberAttribute : AsyncValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext context) =>
        throw new InvalidOperationException("Validate this attribute with IsValidAsync.");

    protected override async Task<ValidationResult?> IsValidAsync(
        object? value, ValidationContext context, CancellationToken cancellationToken)
    {
        var registry = context.GetRequiredService<IVatRegistry>();
        return await registry.IsRegisteredAsync((string?)value, cancellationToken)
            ? ValidationResult.Success
            : new ValidationResult("That VAT number isn't registered with the tax authority.");
    }
}

// 2. A model that validates itself asynchronously.
internal sealed class Invoice : IAsyncValidatableObject
{
    [Required]
    [ValidVatNumber]
    public string? VatNumber { get; set; }

    public decimal Amount { get; set; }

    public async IAsyncEnumerable<ValidationResult> ValidateAsync(
        ValidationContext validationContext,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await Task.Yield();

        if (Amount <= 0m)
        {
            yield return new ValidationResult("Amount must be greater than zero.", [nameof(Amount)]);
        }
    }

    // IAsyncValidatableObject also inherits IValidatableObject;
    // this model only validates
    // asynchronously, so the synchronous path yields nothing.
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) => [];
}

internal interface IVatRegistry
{
    Task<bool> IsRegisteredAsync(string? vatNumber, CancellationToken cancellationToken);
}

internal sealed class FakeVatRegistry : IVatRegistry
{
    private static readonly HashSet<string> Known = ["BE0123456789", "NL001234567B01"];

    public async Task<bool> IsRegisteredAsync(string? vatNumber, CancellationToken cancellationToken)
    {
        // Pretend this is a remote lookup.
        await Task.Delay(25, cancellationToken);
        return vatNumber is not null && Known.Contains(vatNumber);
    }
}
