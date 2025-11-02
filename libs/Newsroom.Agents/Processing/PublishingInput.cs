using Newsroom.Contracts;

namespace Newsroom.Agents.Processing;

public sealed record PublishingInput(Draft Draft, PackagingResult Packaging, DateTimeOffset? ScheduleAt);
