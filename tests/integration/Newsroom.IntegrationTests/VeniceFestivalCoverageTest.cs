using Microsoft.Extensions.Logging.Abstractions;
using Newsroom.Agents.Processing;
using Newsroom.Cms;
using Newsroom.Contracts;
using Xunit;

namespace Newsroom.IntegrationTests;

public sealed class VeniceFestivalCoverageTest
{
    [Fact]
    public async Task Pipeline_Completes_For_Venice_Festival_Request()
    {
        var storyId = StoryId.New();
        var directive = new EditorialDirective(
            storyId,
            "Coverage of the 2025 Venice International Film Festival (82nd edition), winners, notable premieres, sustainability trends in film.",
            "Global culture subscribers",
            9);

        var pitchProcessor = new PitchAgentProcessor();
        var assignmentProcessor = new AssignmentAgentProcessor();
        var reporterProcessor = new ReporterAgentProcessor();
        var factProcessor = new FactCheckAgentProcessor();
        var copyProcessor = new CopyEditAgentProcessor();
        var visualsProcessor = new VisualsAgentProcessor();
        var seoProcessor = new SeopackagingAgentProcessor();
        var monetizationProcessor = new MonetizationAgentProcessor();
        var publishProcessor = new PublishAgentProcessor(new MockPublisher(NullLogger<MockPublisher>.Instance));
        var distributionProcessor = new DistributionAgentProcessor();
        var analyticsProcessor = new AnalyticsAgentProcessor();
        var moderationProcessor = new ModerationAgentProcessor();

        var pitch = await pitchProcessor.ProcessAsync(directive, CancellationToken.None);
        Assert.Equal("venice-film-festival-2025-winners-premieres", pitch.Slug);

        var assignment = await assignmentProcessor.ProcessAsync(pitch, CancellationToken.None);
        var draft = await reporterProcessor.ProcessAsync(assignment, CancellationToken.None);
        var factCheck = await factProcessor.ProcessAsync(draft, CancellationToken.None);
        Assert.True(factCheck.Pass);

        var copyEdit = await copyProcessor.ProcessAsync(new CopyEditInput(draft, factCheck), CancellationToken.None);
        var featuredImage = await visualsProcessor.ProcessAsync(copyEdit, CancellationToken.None);
        var packaging = await seoProcessor.ProcessAsync(new PackagingInput(pitch, copyEdit, featuredImage), CancellationToken.None);
        Assert.Equal("venice-film-festival-2025-winners-premieres", pitch.Slug);
        Assert.Contains("Venice Film Festival 2025", pitch.Keywords);
        Assert.Contains("Golden Lion", pitch.Keywords);
        Assert.Matches(@"https://assets\\.company\\.com/images/.+/feature\\.jpg", packaging.FeaturedImageUrl);

        var monetization = await monetizationProcessor.ProcessAsync(packaging, CancellationToken.None);
        Assert.True(monetization.BehindPaywall);

        var publishOutcome = await publishProcessor.ProcessAsync(new PublishingInput(draft, packaging, null), CancellationToken.None);
        Assert.Equal(storyId, publishOutcome.StoryId);

        var distribution = await distributionProcessor.ProcessAsync(packaging, CancellationToken.None);
        Assert.Contains("Social", distribution.Channels);

        var analyticsEvent = await analyticsProcessor.ProcessAsync(publishOutcome, CancellationToken.None);
        Assert.Equal(storyId, analyticsEvent.StoryId);
        Assert.Equal(storyId.Value.ToString(), analyticsEvent.CorrelationId);

        var moderation = await moderationProcessor.ProcessAsync(packaging, CancellationToken.None);
        Assert.True(moderation.CommentsOpen);
    }
}
