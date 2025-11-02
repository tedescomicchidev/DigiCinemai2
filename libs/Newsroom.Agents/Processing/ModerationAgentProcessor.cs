using Newsroom.Contracts;

namespace Newsroom.Agents.Processing;

public sealed class ModerationAgentProcessor : IAgentProcessor<PackagingResult, ModerationDecision>
{
    public ValueTask<ModerationDecision> ProcessAsync(PackagingResult input, CancellationToken cancellationToken)
    {
        var notes = "Comments open with AI-assisted toxicity filters enabled.";
        return ValueTask.FromResult(new ModerationDecision(input.StoryId, true, notes));
    }
}
