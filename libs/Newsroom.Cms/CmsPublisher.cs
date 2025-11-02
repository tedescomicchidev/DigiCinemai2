using Microsoft.Extensions.Logging;
using Newsroom.Contracts;

namespace Newsroom.Cms;

public interface ICmsPublisher
{
    Task<string> CreateOrUpdateAsync(Draft draft, PackagingResult packaging, CancellationToken cancellationToken);
    Task ScheduleAsync(string cmsId, DateTimeOffset when, CancellationToken cancellationToken);
    Task PublishNowAsync(string cmsId, CancellationToken cancellationToken);
}

public sealed class MockPublisher : ICmsPublisher
{
    private readonly ILogger<MockPublisher> _logger;
    private readonly Dictionary<StoryId, string> _store = new();

    public MockPublisher(ILogger<MockPublisher> logger)
    {
        _logger = logger;
    }

    public Task<string> CreateOrUpdateAsync(Draft draft, PackagingResult packaging, CancellationToken cancellationToken)
    {
        var cmsId = draft.StoryId.Value.ToString();
        _store[draft.StoryId] = cmsId;
        _logger.LogInformation("Upserted story {StoryId} with slug {Slug}", draft.StoryId, packaging.SeoTitle);
        return Task.FromResult(cmsId);
    }

    public Task ScheduleAsync(string cmsId, DateTimeOffset when, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Scheduled {CmsId} at {When}", cmsId, when);
        return Task.CompletedTask;
    }

    public Task PublishNowAsync(string cmsId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Published {CmsId}", cmsId);
        return Task.CompletedTask;
    }
}
