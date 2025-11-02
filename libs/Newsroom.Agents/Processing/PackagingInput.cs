using Newsroom.Contracts;

namespace Newsroom.Agents.Processing;

public sealed record PackagingInput(StoryPitch Pitch, CopyEditResult Copy, string FeaturedImageUrl);
