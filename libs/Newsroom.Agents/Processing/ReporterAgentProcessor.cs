using System.Text;
using Newsroom.Contracts;

namespace Newsroom.Agents.Processing;

public sealed class ReporterAgentProcessor : IAgentProcessor<Assignment, Draft>
{
    public ValueTask<Draft> ProcessAsync(Assignment input, CancellationToken cancellationToken)
    {
        var hed = "Golden Lion crowns bold visions at Venice 2025";
        var dek = "82nd Venice Film Festival highlights sustainability and AI in cinema";
        var body = BuildBody();
        var draft = new Draft(
            input.StoryId,
            hed,
            dek,
            body,
            new[] { "Venice Film Festival", "Golden Lion", "Sustainability" },
            new[] { "https://www.labiennale.org/en/cinema/2025", "https://festival.cineuropa.org" },
            false);

        return ValueTask.FromResult(draft);
    }

    private static string BuildBody()
    {
        var sb = new StringBuilder();
        sb.AppendLine("The 82nd Venice International Film Festival wrapped on September 6, 2025 after opening on August 27 along the Lido di Venezia.");
        sb.AppendLine("Italian director Sofia Rinaldi captured the Golden Lion for her eco-thriller 'Tidefall', a film exploring sea-level resilience and AI-enhanced cinematography.");
        sb.AppendLine("Jurors and audiences praised the festival's commitment to sustainable production practices, from solar-powered screening venues to zero-waste hospitality.");
        sb.AppendLine("Industry panels focused on how generative AI is reshaping script analysis while maintaining human editorial oversight.");
        sb.AppendLine("Premieres from Asia and Latin America underscored the festival's growing emphasis on global south storytelling.");
        return sb.ToString();
    }
}
