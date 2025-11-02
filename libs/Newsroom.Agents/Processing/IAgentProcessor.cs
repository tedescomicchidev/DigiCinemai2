namespace Newsroom.Agents.Processing;

public interface IAgentProcessor<in TInput, TOutput>
{
    ValueTask<TOutput> ProcessAsync(TInput input, CancellationToken cancellationToken);
}
