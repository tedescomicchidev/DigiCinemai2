namespace Newsroom.Contracts;

public enum StoryStage
{
    Pitched,
    Assigned,
    Reporting,
    Drafting,
    FactChecking,
    CopyEdit,
    Packaging,
    ReadyToPublish,
    Scheduled,
    Published,
    Distributed,
    Archived,
    Rework
}

public readonly record struct StoryId(Guid Value)
{
    public static StoryId New() => new(Guid.NewGuid());
    public override string ToString() => Value.ToString();
}

public readonly record struct AssetId(Guid Value)
{
    public static AssetId New() => new(Guid.NewGuid());
    public override string ToString() => Value.ToString();
}

public sealed record EditorialDirective(
    StoryId StoryId,
    string Goal,
    string Audience,
    int Priority
);

public sealed record StoryPitch(
    StoryId StoryId,
    string Slug,
    string HeadlineIdea,
    string Angle,
    string Beat,
    IReadOnlyList<string> Keywords,
    DateTimeOffset? EmbargoUntil,
    IReadOnlyList<string> Sources,
    string? Rationale,
    int Priority
);

public sealed record Assignment(
    StoryId StoryId,
    string Desk,
    string Assignee,
    DateTimeOffset Due,
    IReadOnlyList<string> RequiredAssets
);

public sealed record Draft(
    StoryId StoryId,
    string Hed,
    string Dek,
    string BodyMarkdown,
    IReadOnlyList<string> Tags,
    IReadOnlyList<string> Links,
    bool RequiresLegalReview
);

public sealed record FactCheckRequest(StoryId StoryId, IReadOnlyList<string> Claims, string DraftText);

public sealed record FactCheckResult(StoryId StoryId, bool Pass, IReadOnlyList<string> Flags, string ReportMarkdown);

public sealed record CopyEditRequest(StoryId StoryId, string Text);

public sealed record CopyEditResult(StoryId StoryId, string Text, IReadOnlyList<string> HeadlineVariants);

public sealed record PackagingRequest(StoryId StoryId, string Text, IReadOnlyList<string> Keywords);

public sealed record PackagingResult(StoryId StoryId, string SeoTitle, string SeoDescription, object SchemaOrgJsonLd, string FeaturedImageUrl);

public sealed record PublishRequest(StoryId StoryId, DateTimeOffset? ScheduleAt);

public sealed record DistributionPlan(StoryId StoryId, IReadOnlyList<string> Channels, IReadOnlyList<IReadOnlyList<string>> Posts);

public sealed record MessageEnvelope(
    Guid Id,
    string Type,
    string Role,
    string CorrelationId,
    int Priority,
    DateTimeOffset CreatedAt,
    object Payload
)
{
    public static MessageEnvelope Create<T>(string type, string role, string correlationId, int priority, T payload) =>
        new(Guid.NewGuid(), type, role, correlationId, priority, DateTimeOffset.UtcNow, payload!);
}

public sealed record PublishOutcome(StoryId StoryId, string CmsId, DateTimeOffset? ScheduledAt);

public sealed record MonetizationDecision(StoryId StoryId, bool BehindPaywall, string Rationale);

public sealed record AnalyticsEvent(StoryId StoryId, string Metric, double Value, string CorrelationId);

public sealed record ModerationDecision(StoryId StoryId, bool CommentsOpen, string Notes);
