using FluentValidation;

namespace Newsroom.Contracts.Validation;

public sealed class EditorialDirectiveValidator : AbstractValidator<EditorialDirective>
{
    public EditorialDirectiveValidator()
    {
        RuleFor(d => d.StoryId).Must(id => id.Value != Guid.Empty, "StoryId must be a non-empty GUID");
        RuleFor(d => d.Goal).NotEmpty();
        RuleFor(d => d.Audience).NotEmpty();
        RuleFor(d => d.Priority).GreaterThanOrEqualTo(0).LessThanOrEqualTo(10);
    }
}

public sealed class StoryPitchValidator : AbstractValidator<StoryPitch>
{
    public StoryPitchValidator()
    {
        RuleFor(p => p.StoryId).Must(id => id.Value != Guid.Empty, "StoryId must be a non-empty GUID");
        RuleFor(p => p.Slug).NotEmpty().MaximumLength(120);
        RuleFor(p => p.HeadlineIdea).NotEmpty().MaximumLength(180);
        RuleFor(p => p.Angle).NotEmpty();
        RuleFor(p => p.Beat).NotEmpty();
        RuleFor(p => p.Priority).GreaterThanOrEqualTo(0).LessThanOrEqualTo(10);
        RuleFor(p => p.Keywords).NotNull("Keywords must be provided").Must(k => k.Count <= 12, "Keywords must contain 12 items or fewer");
        RuleFor(p => p.Sources).NotNull("Sources must be provided");
    }
}

public sealed class AssignmentValidator : AbstractValidator<Assignment>
{
    public AssignmentValidator()
    {
        RuleFor(a => a.StoryId).Must(id => id.Value != Guid.Empty, "StoryId must be a non-empty GUID");
        RuleFor(a => a.Desk).NotEmpty();
        RuleFor(a => a.Assignee).NotEmpty();
        RuleFor(a => a.RequiredAssets).NotNull("Required assets must be provided");
    }
}

public sealed class DraftValidator : AbstractValidator<Draft>
{
    public DraftValidator()
    {
        RuleFor(d => d.StoryId).Must(id => id.Value != Guid.Empty, "StoryId must be a non-empty GUID");
        RuleFor(d => d.Hed).NotEmpty();
        RuleFor(d => d.Dek).NotEmpty();
        RuleFor(d => d.BodyMarkdown).NotEmpty();
        RuleFor(d => d.Tags).NotNull("Tags collection must be provided");
    }
}

public sealed class FactCheckRequestValidator : AbstractValidator<FactCheckRequest>
{
    public FactCheckRequestValidator()
    {
        RuleFor(f => f.StoryId).Must(id => id.Value != Guid.Empty, "StoryId must be a non-empty GUID");
        RuleFor(f => f.Claims).NotNull("Claims must be provided").MinimumLength(1);
        RuleFor(f => f.DraftText).NotEmpty();
    }
}

public sealed class FactCheckResultValidator : AbstractValidator<FactCheckResult>
{
    public FactCheckResultValidator()
    {
        RuleFor(f => f.StoryId).Must(id => id.Value != Guid.Empty, "StoryId must be a non-empty GUID");
        RuleFor(f => f.ReportMarkdown).NotEmpty();
    }
}

public sealed class CopyEditRequestValidator : AbstractValidator<CopyEditRequest>
{
    public CopyEditRequestValidator()
    {
        RuleFor(c => c.StoryId).Must(id => id.Value != Guid.Empty, "StoryId must be a non-empty GUID");
        RuleFor(c => c.Text).NotEmpty();
    }
}

public sealed class CopyEditResultValidator : AbstractValidator<CopyEditResult>
{
    public CopyEditResultValidator()
    {
        RuleFor(c => c.StoryId).Must(id => id.Value != Guid.Empty, "StoryId must be a non-empty GUID");
        RuleFor(c => c.Text).NotEmpty();
        RuleFor(c => c.HeadlineVariants).NotNull("Headline variants must be provided").MinimumLength(1);
    }
}

public sealed class PackagingRequestValidator : AbstractValidator<PackagingRequest>
{
    public PackagingRequestValidator()
    {
        RuleFor(p => p.StoryId).Must(id => id.Value != Guid.Empty, "StoryId must be a non-empty GUID");
        RuleFor(p => p.Text).NotEmpty();
        RuleFor(p => p.Keywords).NotNull("Keywords must be provided").MinimumLength(1);
    }
}

public sealed class PackagingResultValidator : AbstractValidator<PackagingResult>
{
    public PackagingResultValidator()
    {
        RuleFor(p => p.StoryId).Must(id => id.Value != Guid.Empty, "StoryId must be a non-empty GUID");
        RuleFor(p => p.SeoTitle).NotEmpty();
        RuleFor(p => p.SeoDescription).NotEmpty();
        RuleFor(p => p.FeaturedImageUrl).NotEmpty();
    }
}

public sealed class PublishRequestValidator : AbstractValidator<PublishRequest>
{
    public PublishRequestValidator()
    {
        RuleFor(p => p.StoryId).Must(id => id.Value != Guid.Empty, "StoryId must be a non-empty GUID");
    }
}

public sealed class PublishOutcomeValidator : AbstractValidator<PublishOutcome>
{
    public PublishOutcomeValidator()
    {
        RuleFor(p => p.StoryId).Must(id => id.Value != Guid.Empty, "StoryId must be a non-empty GUID");
        RuleFor(p => p.CmsId).NotEmpty();
    }
}

public sealed class DistributionPlanValidator : AbstractValidator<DistributionPlan>
{
    public DistributionPlanValidator()
    {
        RuleFor(p => p.StoryId).Must(id => id.Value != Guid.Empty, "StoryId must be a non-empty GUID");
        RuleFor(p => p.Channels).NotNull("Channels must be provided").MinimumLength(1);
        RuleFor(p => p.Posts).NotNull("Posts must be provided");
    }
}

public sealed class MessageEnvelopeValidator : AbstractValidator<MessageEnvelope>
{
    public MessageEnvelopeValidator()
    {
        RuleFor(e => e.Id).Must(id => id != Guid.Empty, "Envelope Id must be a non-empty GUID");
        RuleFor(e => e.Type).NotEmpty();
        RuleFor(e => e.Role).NotEmpty();
        RuleFor(e => e.CorrelationId).NotEmpty();
        RuleFor(e => e.Payload).NotNull("Payload is required");
    }
}

public sealed class MonetizationDecisionValidator : AbstractValidator<MonetizationDecision>
{
    public MonetizationDecisionValidator()
    {
        RuleFor(m => m.StoryId).Must(id => id.Value != Guid.Empty, "StoryId must be a non-empty GUID");
        RuleFor(m => m.Rationale).NotEmpty();
    }
}

public sealed class AnalyticsEventValidator : AbstractValidator<AnalyticsEvent>
{
    public AnalyticsEventValidator()
    {
        RuleFor(a => a.StoryId).Must(id => id.Value != Guid.Empty, "StoryId must be a non-empty GUID");
        RuleFor(a => a.Metric).NotEmpty();
        RuleFor(a => a.CorrelationId).NotEmpty();
    }
}

public sealed class ModerationDecisionValidator : AbstractValidator<ModerationDecision>
{
    public ModerationDecisionValidator()
    {
        RuleFor(m => m.StoryId).Must(id => id.Value != Guid.Empty, "StoryId must be a non-empty GUID");
        RuleFor(m => m.Notes).NotEmpty();
    }
}
