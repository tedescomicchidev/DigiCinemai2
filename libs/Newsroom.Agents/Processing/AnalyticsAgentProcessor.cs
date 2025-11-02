using Newsroom.Contracts;

namespace Newsroom.Agents.Processing;

public sealed class AnalyticsAgentProcessor : IAgentProcessor<PublishOutcome, AnalyticsEvent>
{
    public ValueTask<AnalyticsEvent> ProcessAsync(PublishOutcome input, CancellationToken cancellationToken)
    {
        var evt = new AnalyticsEvent(input.StoryId, "StoryPublished", 1, input.StoryId.Value.ToString());
        return ValueTask.FromResult(evt);
    }
}
