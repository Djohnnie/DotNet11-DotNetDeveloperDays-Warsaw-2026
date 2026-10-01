// C# 15 introduces union types, which represent a value that can be one of several case types.
// Declare a union with the union keyword.


// Union conversions:
// An implicit union conversion exists from each case type to the union type.
// You don't need to call a constructor explicitly:
static void BasicConversion()
{
    PaymentMethod payment1 = new CreditCard("1234-5678-9012-3456", "12/24", "123");
    Console.WriteLine(payment1.Value); // output: CreditCard { MaskedNumber = 1234-5678-9012-3456, Expiry = 12/24, Cvv = 123 }

    PaymentMethod payment2 = new PayPal("whiskers@example.com");
    Console.WriteLine(payment2.Value); // output: PayPal { Email = whiskers@example.com }
}


// Union matching:
// When you pattern match on a union type, patterns apply to the union's Value property,
// not the union value itself. This "unwrapping" behavior means the union is transparent to pattern matching:
static void PatternMatching()
{
    PaymentMethod payment = new CreditCard("1234-5678-9012-3456", "12/24", "123");

    var identifier = payment switch
    {
        CreditCard credit => credit.MaskedNumber,
        BankTransfer bank => bank.Iban,
        PayPal paypal => paypal.Email,
        Crypto crypto => crypto.WalletAddress,
        null => "no payment method"
    };
}


union OrderEvent(OrderPlaced, OrderShipped, OrderCancelled, PaymentFailed);
record OrderPlaced(Guid OrderId, string Customer, decimal Total);
record OrderShipped(Guid OrderId, string TrackingCode, DateTime ShippedAt);
record OrderCancelled(Guid OrderId, string Reason);
record PaymentFailed(Guid OrderId, string ProviderError);


union PaymentMethod(CreditCard, BankTransfer, PayPal, Crypto);
record CreditCard(string MaskedNumber, string Expiry, string Cvv);
record BankTransfer(string Iban, string Bic);
record PayPal(string Email);
record Crypto(string WalletAddress, string Coin);



// In .NET 11 Preview 2, these types aren't included in the runtime.
// To use union types, you must declare them in your project.
// They'll be included in a future .NET preview.
// The following attribute and interface support union types at compile time and runtime:
namespace System.Runtime.CompilerServices
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false)]
    public sealed class UnionAttribute : Attribute;

    public interface IUnion
    {
        object? Value { get; }
    }
}