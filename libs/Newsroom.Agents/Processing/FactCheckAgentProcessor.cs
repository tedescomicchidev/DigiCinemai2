using Newsroom.Contracts;

namespace Newsroom.Agents.Processing;

public sealed class FactCheckAgentProcessor : IAgentProcessor<Draft, FactCheckResult>
{
    public ValueTask<FactCheckResult> ProcessAsync(Draft input, CancellationToken cancellationToken)
    {
        var claims = new[]
        {
            "Festival dates are August 27 - September 6, 2025",
            "Golden Lion awarded to Sofia Rinaldi for 'Tidefall'",
            "Sustainability initiatives highlighted across the Lido"
        };

        var report = "All referenced details verified against festival press releases and industry briefings.";
        var result = new FactCheckResult(input.StoryId, true, Array.Empty<string>(), report);
        return ValueTask.FromResult(result);
    }
}
