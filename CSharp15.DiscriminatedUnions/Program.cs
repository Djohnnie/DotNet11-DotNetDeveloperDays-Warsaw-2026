// C# 15's headline language feature: union types (a.k.a. discriminated unions).
// A union declares that a value must be exactly one of a fixed, closed set of "case" types.
// This spans several .NET 11 previews:
//  - Preview 3: initial "union type support"
//  - Preview 5: "Closed Class Hierarchies" + "Union Declarations/Patterns"
//  - Preview 6: "Unions ship their support types in the box" (no more hand-rolled UnionAttribute/IUnion,
//               unlike the early dotnet10/CSharp15.UnionTypeSupport preview stub) and
//               "System.Text.Json serializes C# union types"
//  - Preview 7: "Union Pattern Matching" now matches case types directly (no need to unwrap a .Value
//               property anymore) and "Exhaustiveness for Constrained Type Parameters"
//
// Declare a union with the `union` keyword, listing its allowed case types in parentheses.
// Each case type implicitly converts to the union type - no explicit constructor call needed.

using System.Text.Json;

OrderEvent placed = new OrderPlaced(Guid.NewGuid(), "Ada Lovelace", 129.90m);
OrderEvent shipped = new OrderShipped(placed is OrderPlaced op ? op.OrderId : Guid.Empty, "1Z-999-AA1", DateTime.UtcNow);

Describe(placed);
Describe(shipped);
Describe(new OrderCancelled(Guid.NewGuid(), "Customer changed their mind"));
Describe(new PaymentFailed(Guid.NewGuid(), "Card declined"));

Console.WriteLine();
Console.WriteLine("Serialized union payload:");
Console.WriteLine(JsonSerializer.Serialize(placed));

// A related, separate feature: "closed class hierarchies". A `closed class` restricts which types
// are allowed to derive from it to those declared in the SAME compilation - similar in spirit to a
// union, but for a real inheritance hierarchy (with shared members) rather than a flat case set.
// The compiler can apply the same exhaustiveness analysis to pattern matches over a closed hierarchy.
Shape shape = new Circle(radius: 5);
Console.WriteLine();
Console.WriteLine(Area(shape).ToString("F2"));

// Pattern matching on a union matches the CASE TYPES directly - the union is "transparent" to `switch`.
// Because OrderEvent is a closed set of exactly four case types, the compiler can PROVE this switch
// is exhaustive, so no `default` arm (and no CS8509 warning) is required:
static void Describe(OrderEvent orderEvent) => Console.WriteLine(orderEvent switch
{
    OrderPlaced(var id, var customer, var total) => $"Order {id} placed by {customer} for {total:C}",
    OrderShipped(var id, var tracking, var shippedAt) => $"Order {id} shipped ({tracking}) at {shippedAt:u}",
    OrderCancelled(var id, var reason) => $"Order {id} cancelled: {reason}",
    PaymentFailed(var id, var error) => $"Order {id} payment failed: {error}",
});

// If you delete a case arm above, or add a new case type to the union below without handling it
// anywhere, the compiler flags it immediately with warning CS8509 ("switch expression does not
// handle all possible values ... it is not exhaustive") instead of letting a case silently fall
// through to a runtime exception. Try it: comment out the PaymentFailed arm above and rebuild.

static double Area(Shape shape) => shape switch
{
    Circle c => Math.PI * c.Radius * c.Radius,
    Square s => s.Side * s.Side,
    // No default needed: Circle and Square are the only types permitted to derive from `closed class Shape`.
};

union OrderEvent(OrderPlaced, OrderShipped, OrderCancelled, PaymentFailed);
record OrderPlaced(Guid OrderId, string Customer, decimal Total);
record OrderShipped(Guid OrderId, string TrackingCode, DateTime ShippedAt);
record OrderCancelled(Guid OrderId, string Reason);
record PaymentFailed(Guid OrderId, string ProviderError);

closed class Shape;
class Circle(double radius) : Shape { public double Radius => radius; }
class Square(double side) : Shape { public double Side => side; }
