using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newsroom.Contracts;

namespace Newsroom.Agents;

public abstract class AgentBase : BackgroundService
{
    private readonly ILogger _logger;
    private readonly IMessageClient _messageClient;

    protected AgentBase(ILogger logger, IMessageClient messageClient)
    {
        _logger = logger;
        _messageClient = messageClient;
    }

    protected abstract string SubscriptionName { get; }

    protected virtual TimeSpan PollInterval => TimeSpan.FromSeconds(1);

    protected virtual ValueTask HandleErrorAsync(Exception exception, CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Unhandled exception in {Agent}", GetType().Name);
        return ValueTask.CompletedTask;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("{Agent} starting", GetType().Name);
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var envelope = await _messageClient.ReceiveAsync(SubscriptionName, stoppingToken).ConfigureAwait(false);
                if (envelope is not null)
                {
                    await HandleAsync(envelope, stoppingToken).ConfigureAwait(false);
                }
                else
                {
                    await Task.Delay(PollInterval, stoppingToken).ConfigureAwait(false);
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex, stoppingToken).ConfigureAwait(false);
                await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken).ConfigureAwait(false);
            }
        }

        _logger.LogInformation("{Agent} stopping", GetType().Name);
    }

    protected abstract Task HandleAsync(MessageEnvelope envelope, CancellationToken cancellationToken);

    protected Task PublishAsync(string topic, object payload, MessageEnvelope? source, CancellationToken cancellationToken)
    {
        var envelope = MessageEnvelope.Create(type: payload.GetType().Name, role: GetType().Name, correlationId: source?.CorrelationId ?? Guid.NewGuid().ToString(), priority: source?.Priority ?? 1, payload: payload);
        return _messageClient.PublishAsync(topic, envelope, cancellationToken);
    }
}

public interface IMessageClient
{
    Task PublishAsync(string topic, MessageEnvelope envelope, CancellationToken cancellationToken);
    Task<MessageEnvelope?> ReceiveAsync(string subscriptionName, CancellationToken cancellationToken);
}

public static class AgentServiceCollectionExtensions
{
    public static IServiceCollection AddAgentInfrastructure<TAgent>(this IServiceCollection services)
        where TAgent : class, IHostedService
    {
        services.AddHostedService<TAgent>();
        return services;
    }
}
