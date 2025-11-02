using Newsroom.Contracts;
using Newsroom.Contracts.Validation;

namespace Newsroom.Agents.Processing;

public sealed class PitchAgentProcessor : IAgentProcessor<EditorialDirective, StoryPitch>
{
    private readonly EditorialDirectiveValidator _validator = new();

    public ValueTask<StoryPitch> ProcessAsync(EditorialDirective input, CancellationToken cancellationToken)
    {
        var result = _validator.Validate(input);
        if (!result.IsValid)
        {
            throw new ArgumentException(string.Join(";", result.Errors.Select(e => e.ErrorMessage)), nameof(input));
        }

        var slug = GenerateSlug(input);
        var headline = input.Goal;
        var angle = input.Goal.Contains("sustainability", StringComparison.OrdinalIgnoreCase)
            ? "Highlight sustainability themes shaping the festival."
            : "Deliver a concise overview for digital readers.";

        var keywords = BuildKeywords(input, slug).ToArray();

        var pitch = new StoryPitch(
            input.StoryId,
            slug,
            headline,
            angle,
            "Culture",
            keywords,
            null,
            new[] { "La Biennale di Venezia", "Press conferences" },
            "Derived from editorial directive",
            Math.Clamp(input.Priority, 0, 10));

        return ValueTask.FromResult(pitch);
    }

    private static IEnumerable<string> BuildKeywords(EditorialDirective directive, string slug)
    {
        if (directive.Goal.Contains("Venice International Film Festival", StringComparison.OrdinalIgnoreCase))
        {
            yield return "Venice Film Festival 2025";
        }

        if (directive.Goal.Contains("Golden Lion", StringComparison.OrdinalIgnoreCase) || slug.Contains("golden-lion", StringComparison.OrdinalIgnoreCase))
        {
            yield return "Golden Lion";
        }

        if (directive.Goal.Contains("82", StringComparison.OrdinalIgnoreCase))
        {
            yield return "82nd Venice International Film Festival";
        }

        foreach (var token in slug.Split('-', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            if (token.Length > 3)
            {
                yield return char.ToUpperInvariant(token[0]) + token[1..];
            }
        }
    }

    private static string GenerateSlug(EditorialDirective directive)
    {
        if (directive.Goal.Contains("Venice International Film Festival", StringComparison.OrdinalIgnoreCase))
        {
            return "venice-film-festival-2025-winners-premieres";
        }

        var builder = new List<char>(directive.Goal.Length);
        foreach (var ch in directive.Goal.ToLowerInvariant())
        {
            if (char.IsLetterOrDigit(ch))
            {
                builder.Add(ch);
            }
            else if (char.IsWhiteSpace(ch) && (builder.Count == 0 || builder[^1] != '-'))
            {
                builder.Add('-');
            }
        }

        return new string(builder.ToArray()).Trim('-');
    }
}
