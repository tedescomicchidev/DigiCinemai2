using Newsroom.Contracts;

namespace Newsroom.Agents.Processing;

public sealed class SeopackagingAgentProcessor : IAgentProcessor<PackagingInput, PackagingResult>
{
    public ValueTask<PackagingResult> ProcessAsync(PackagingInput input, CancellationToken cancellationToken)
    {
        var seoTitle = input.Copy.HeadlineVariants.First();
        var seoDescription = "Golden Lion goes to 'Tidefall' as Venice Film Festival spotlights sustainable cinema.";
        var schema = new
        {
            @context = "https://schema.org",
            @type = "NewsArticle",
            headline = seoTitle,
            image = input.FeaturedImageUrl,
            keywords = input.Pitch.Keywords,
            description = seoDescription
        };

        var result = new PackagingResult(
            input.Pitch.StoryId,
            seoTitle,
            seoDescription,
            schema,
            input.FeaturedImageUrl);

        return ValueTask.FromResult(result);
    }
}
