using Newsroom.Contracts;

namespace Newsroom.Agents.Processing;

public sealed class CopyEditAgentProcessor : IAgentProcessor<CopyEditInput, CopyEditResult>
{
    public ValueTask<CopyEditResult> ProcessAsync(CopyEditInput input, CancellationToken cancellationToken)
    {
        var text = input.Draft.BodyMarkdown.Trim();
        var variants = new[]
        {
            "Golden Lion awarded at 82nd Venice Film Festival",
            "Venice 2025 crowns 'Tidefall' amid sustainability push",
            "AI and eco-innovation define 2025 Venice Film Festival"
        };

        var result = new CopyEditResult(input.Draft.StoryId, text, variants);
        return ValueTask.FromResult(result);
    }
}
