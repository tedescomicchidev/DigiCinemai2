using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Newsroom.Contracts;

namespace Newsroom.Agents.Messaging;

public sealed class InMemoryMessageClient : IMessageClient
{
    private readonly ConcurrentDictionary<string, ConcurrentQueue<MessageEnvelope>> _queues = new();
    private readonly ILogger<InMemoryMessageClient> _logger;

    public InMemoryMessageClient(ILogger<InMemoryMessageClient> logger)
    {
        _logger = logger;
    }

    public Task PublishAsync(string topic, MessageEnvelope envelope, CancellationToken cancellationToken)
    {
        var queue = _queues.GetOrAdd(topic, _ => new ConcurrentQueue<MessageEnvelope>());
        queue.Enqueue(envelope);
        _logger.LogInformation("Published {Type} to {Topic}", envelope.Type, topic);
        return Task.CompletedTask;
    }

    public Task<MessageEnvelope?> ReceiveAsync(string subscriptionName, CancellationToken cancellationToken)
    {
        if (_queues.TryGetValue(subscriptionName, out var queue) && queue.TryDequeue(out var envelope))
        {
            _logger.LogInformation("Dequeued {Type} from {Subscription}", envelope.Type, subscriptionName);
            return Task.FromResult<MessageEnvelope?>(envelope);
        }

        return Task.FromResult<MessageEnvelope?>(null);
    }
}
