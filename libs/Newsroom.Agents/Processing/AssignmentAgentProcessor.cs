using Newsroom.Contracts;

namespace Newsroom.Agents.Processing;

public sealed class AssignmentAgentProcessor : IAgentProcessor<StoryPitch, Assignment>
{
    public ValueTask<Assignment> ProcessAsync(StoryPitch input, CancellationToken cancellationToken)
    {
        var desk = input.Beat.Contains("culture", StringComparison.OrdinalIgnoreCase) ? "Culture Desk" : "News Desk";
        var assignment = new Assignment(
            input.StoryId,
            desk,
            "Elena Marino",
            DateTimeOffset.UtcNow.AddHours(6),
            new[] { "feature-image", "quote-pull" });

        return ValueTask.FromResult(assignment);
    }
}
