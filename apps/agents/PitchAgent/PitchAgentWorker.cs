using Microsoft.Extensions.Logging;
using Newsroom.Agents;
using Newsroom.Agents.Processing;
using Newsroom.Contracts;

namespace PitchAgent;

public sealed class PitchAgentWorker : AgentBase
{
    private readonly ILogger<PitchAgentWorker> _logger;
    private readonly PitchAgentProcessor _processor;

    public PitchAgentWorker(ILogger<PitchAgentWorker> logger, IMessageClient messageClient, PitchAgentProcessor processor)
        : base(logger, messageClient)
    {
        _logger = logger;
        _processor = processor;
    }

    protected override string SubscriptionName => Topics.Pitches;

    protected override async Task HandleAsync(MessageEnvelope envelope, CancellationToken cancellationToken)
    {
        if (envelope.Payload is EditorialDirective directive)
        {
            var pitch = await _processor.ProcessAsync(directive, cancellationToken).ConfigureAwait(false);
            _logger.LogInformation("Generated pitch {Slug}", pitch.Slug);
            await PublishAsync(Topics.Assignments, pitch, envelope, cancellationToken).ConfigureAwait(false);
            return;
        }

        _logger.LogWarning("Unexpected payload type {Type}", envelope.Payload.GetType().Name);
    }
}
