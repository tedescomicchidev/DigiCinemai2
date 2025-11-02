using Newsroom.Cms;
using Newsroom.Contracts;

namespace Newsroom.Agents.Processing;

public sealed class PublishAgentProcessor : IAgentProcessor<PublishingInput, PublishOutcome>
{
    private readonly ICmsPublisher _publisher;

    public PublishAgentProcessor(ICmsPublisher publisher)
    {
        _publisher = publisher;
    }

    public async ValueTask<PublishOutcome> ProcessAsync(PublishingInput input, CancellationToken cancellationToken)
    {
        var cmsId = await _publisher.CreateOrUpdateAsync(input.Draft, input.Packaging, cancellationToken).ConfigureAwait(false);
        if (input.ScheduleAt.HasValue)
        {
            await _publisher.ScheduleAsync(cmsId, input.ScheduleAt.Value, cancellationToken).ConfigureAwait(false);
        }
        else
        {
            await _publisher.PublishNowAsync(cmsId, cancellationToken).ConfigureAwait(false);
        }

        return new PublishOutcome(input.Draft.StoryId, cmsId, input.ScheduleAt);
    }
}
