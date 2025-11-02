using Newsroom.Contracts;

namespace Newsroom.Agents.Processing;

public sealed class MonetizationAgentProcessor : IAgentProcessor<PackagingResult, MonetizationDecision>
{
    public ValueTask<MonetizationDecision> ProcessAsync(PackagingResult input, CancellationToken cancellationToken)
    {
        var behindPaywall = input.SeoTitle.Contains("Golden Lion", StringComparison.OrdinalIgnoreCase);
        var rationale = behindPaywall
            ? "High interest awards coverage gated for subscribers after 3 free views."
            : "Open access evergreen.";
        return ValueTask.FromResult(new MonetizationDecision(input.StoryId, behindPaywall, rationale));
    }
}
