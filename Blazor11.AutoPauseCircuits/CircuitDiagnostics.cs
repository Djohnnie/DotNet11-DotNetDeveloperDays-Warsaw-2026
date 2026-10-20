using Microsoft.AspNetCore.Components.Server.Circuits;

namespace Blazor11.AutoPauseCircuits;

public sealed class CircuitDiagnostics(ILogger<CircuitDiagnostics> logger) : CircuitHandler
{
    public string? CircuitId { get; private set; }
    public bool IsConnected { get; private set; }
    public int ConnectionUpCount { get; private set; }
    public int ConnectionDownCount { get; private set; }
    public DateTimeOffset? LastConnectedAt { get; private set; }
    public DateTimeOffset? LastDisconnectedAt { get; private set; }

    public event Func<Task>? Changed;

    public override async Task OnCircuitOpenedAsync(Circuit circuit, CancellationToken cancellationToken)
    {
        CircuitId = circuit.Id;
        logger.LogInformation("Circuit {CircuitId} opened", circuit.Id);
        await NotifyChangedAsync();
    }

    public override async Task OnConnectionUpAsync(Circuit circuit, CancellationToken cancellationToken)
    {
        IsConnected = true;
        ConnectionUpCount++;
        LastConnectedAt = DateTimeOffset.UtcNow;
        logger.LogInformation("Circuit {CircuitId} connection up (#{ConnectionUpCount})",
            circuit.Id, ConnectionUpCount);
        await NotifyChangedAsync();
    }

    public override async Task OnConnectionDownAsync(Circuit circuit, CancellationToken cancellationToken)
    {
        IsConnected = false;
        ConnectionDownCount++;
        LastDisconnectedAt = DateTimeOffset.UtcNow;
        logger.LogInformation("Circuit {CircuitId} connection down (#{ConnectionDownCount})",
            circuit.Id, ConnectionDownCount);
        await NotifyChangedAsync();
    }

    public override Task OnCircuitClosedAsync(Circuit circuit, CancellationToken cancellationToken)
    {
        logger.LogInformation("Circuit {CircuitId} closed", circuit.Id);
        return Task.CompletedTask;
    }

    private async Task NotifyChangedAsync()
    {
        if (Changed is { } changed)
        {
            foreach (Func<Task> handler in changed.GetInvocationList())
            {
                await handler();
            }
        }
    }
}
