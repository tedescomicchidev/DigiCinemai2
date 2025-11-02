using Newsroom.Contracts;

namespace Newsroom.Agents.Processing;

public sealed class VisualsAgentProcessor : IAgentProcessor<CopyEditResult, string>
{
    public ValueTask<string> ProcessAsync(CopyEditResult input, CancellationToken cancellationToken)
    {
        var slug = input.HeadlineVariants.FirstOrDefault()?.ToLowerInvariant().Replace(' ', '-') ?? "story";
        var url = $"https://assets.company.com/images/{slug}/feature.jpg";
        return ValueTask.FromResult(url);
    }
}
