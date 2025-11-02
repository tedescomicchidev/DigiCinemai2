using Newsroom.Contracts;

namespace Newsroom.Agents.Processing;

public sealed class DistributionAgentProcessor : IAgentProcessor<PackagingResult, DistributionPlan>
{
    public ValueTask<DistributionPlan> ProcessAsync(PackagingResult input, CancellationToken cancellationToken)
    {
        var channels = new[] { "Homepage", "Social", "Newsletter" };
        var posts = new[]
        {
            new[] { "X", $"Golden Lion winner 'Tidefall' headlines Venice 2025. {input.FeaturedImageUrl}" },
            new[] { "LinkedIn", "Venice Film Festival spotlights sustainability and AI-led production." },
            new[] { "Newsletter", "Tonight's top story: Venice crowns a new Golden Lion winner." }
        };

        var plan = new DistributionPlan(input.StoryId, channels, posts);
        return ValueTask.FromResult(plan);
    }
}
