# Newsroom Specification

## Prompt for OpenAI Codex – Azure Multi-Agent .NET Newsroom Platform (v2)

**Goal:**  
Build a production-ready, multi-agent .NET platform on Azure that automates end-to-end digital newspaper operations with minimal human intervention. A human “Editor-in-Chief” uses a web front end to provide direction; autonomous agents built with the Microsoft Agent Framework operate as independent services, and the underlying application architecture uses .NET Aspire.

---

## Inputs  
- `newsroom_spec`: A detailed description of how a modern digital newspaper office operates (editorial workflow, CMS usage, roles, publishing/distribution, analytics/monetization, tech stack, daily ops, challenges/innovations). Use the text supplied by the user.  
- Place the value of `newsroom_spec` verbatim in `docs/newsroom_spec.md`, and derive requirements from it.

---

## What to Build (High Level)  
Create a monorepo with:

1. **Web Front End**: ASP.NET Core + Blazor application (`/apps/editor-portal`) where the Editor-in-Chief can:
   - Define coverage priorities, create and approve pitches, set embargoes.
   - Configure paywall rules, SEO preferences, distribution channels.
   - Monitor live workflows/orchestrations, approve escalations, view analytics dashboards.

2. **Agent Services** (containerised .NET 8+ microservices or Azure Functions) running on Azure using independent deployables:
   - `PitchAgent` – ingests EIC goals and wire/feeds; proposes story pitches.
   - `AssignmentAgent` – assigns stories to desks/reporters, sets timelines and assets.
   - `ReporterAgent` – drafts stories (LLM-assisted), requests assets/transcripts.
   - `FactCheckAgent` – verifies claims, sources, citations; flags risk.
   - `CopyEditAgent` – applies style guide, refines headline/deck variants.
   - `SEOPackagingAgent` – keywords, meta tags, schema.org JSON-LD, recirculation modules.
   - `VisualsAgent` – selects/crops featured image, creates simple charts or maps via stubbed API.
   - `PublishAgent` – integrates with CMS abstraction, schedules or publishes content.
   - `DistributionAgent` – handles social posts, push alerts, newsletter slots.
   - `MonetizationAgent` – paywall logic, subscription triggers, ad ops hook.
   - `AnalyticsAgent` – gathers and analyses engagement metrics, triggers A/B tests.
   - `ModerationAgent` – user comments and UGC verification, safety filtering.

3. **Orchestration Layer**: Use the Microsoft Agent Framework workflow support (orchestrator pattern) or Azure Durable Functions for story pipelines: state machine/saga that moves a story through stages, triggers agents, handles human-in-loop, error handling, compensation.

4. **Shared Libraries**:  
   - `/libs/Newsroom.Contracts` – domain models, message schemas.  
   - `/libs/Newsroom.Agents` – base agent classes, infrastructure for pub/sub, telemetry, correlation.  
   - `/libs/Newsroom.Cms` – CMS interface and adapters.

5. **Infrastructure as Code (IaC)**: Bicep or Terraform under `/infra/` to provision Azure resources.

6. **CI/CD**: GitHub Actions workflows for build, test, containerisation, deploy to Azure.

---

## Azure Architecture  
- **Compute**: Azure Container Apps (ACA) or AKS for containerised agents; Azure Functions for serverless agents if preferred.  
- **Messaging**: Azure Service Bus (topics/subscriptions) or Dapr pub/sub for agent communication.  
- **Storage/Databases**:  
  - Azure Cosmos DB (NoSQL) or Azure Database for PostgreSQL for content metadata and orchestration state.  
  - Azure Blob Storage for assets.  
  - Azure Redis Cache for caching and queues.  
  - Azure Cognitive Search for content search/indexing.  
- **AI/LLM**: Azure OpenAI for text generation, summarisation, metadata extraction. Azure Content Safety for moderation.  
- **Identity & Security**: Microsoft Entra ID for auth, RBAC based on roles (`EditorInChief`, `ManagingEditor`, etc.). Azure Key Vault for secrets, Front Door + WAF + CDN for delivery.  
- **Observability**: Azure Application Insights, Log Analytics, dashboards and Aspire’s built-in telemetry.  
- **Delivery**: Azure Front Door + Azure CDN for global reach.

---

## Domain Model & Contracts  
Under `/libs/Newsroom.Contracts`, define C# records and JSON schemas:

```csharp
public enum StoryStage {
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

public record StoryId(Guid Value);
public record AssetId(Guid Value);

public record StoryPitch(
    StoryId StoryId,
    string Slug,
    string HeadlineIdea,
    string Angle,
    string Beat,
    string[] Keywords,
    DateTimeOffset? EmbargoUntil,
    string[] Sources,
    string? Rationale,
    int Priority
);

public record Assignment(
    StoryId StoryId,
    string Desk,
    string Assignee,
    DateTimeOffset Due,
    string[] RequiredAssets
);

public record Draft(
    StoryId StoryId,
    string Hed,
    string Dek,
    string BodyMarkdown,
    string[] Tags,
    string[] Links,
    bool RequiresLegalReview
);

public record FactCheckRequest(StoryId StoryId, string[] Claims, string DraftText);
public record FactCheckResult(StoryId StoryId, bool Pass, string[] Flags, string ReportMarkdown);

public record CopyEditRequest(StoryId StoryId, string Text);
public record CopyEditResult(StoryId StoryId, string Text, string[] HeadlineVariants);

public record PackagingRequest(StoryId StoryId, string Text, string[] Keywords);
public record PackagingResult(StoryId StoryId, string SeoTitle, string SeoDescription, object SchemaOrgJsonLd, string FeaturedImageUrl);

public record PublishRequest(StoryId StoryId, DateTimeOffset? ScheduleAt);
public record DistributionPlan(StoryId StoryId, string[] Channels, string[][] Posts);

public record MessageEnvelope(
    Guid Id,
    string Type,
    string Role,
    string CorrelationId,
    int Priority,
    DateTimeOffset CreatedAt,
    object Payload
);

Add JSON schemas in ./schemas/ and validators using FluentValidation.
```

---

Messaging & Pub-Sub Topology

Define topics (or Dapr pub/sub channels) such as:

pitches

assignments

drafts

factcheck

copyedit

packaging

publish

distribute

analytics

moderation

errors

Each agent subscribes to a topic, processes the message, then publishes a result to the next topic. Use correlation IDs, dead-letter handling and poison message routing.

---

Orchestration / Workflow

Using Microsoft Agent Framework workflows or Azure Durable Functions:

StoryOrchestrator (instance per story):

1. Receive StoryPitch.

2. Trigger AssignmentAgent.

3. Wait for Assignment.

4. Trigger ReporterAgent → FactCheckAgent → approval if needed → CopyEditAgent → SEOPackagingAgent.

5. Trigger PublishAgent → DistributionAgent.

6. Notify AnalyticsAgent and record completion.

Support timeout, error compensation, human override (Editor-in-Chief approval).

---

Agents Implementation

Base library (/libs/Newsroom.Agents) provides AgentBase with:

Messaging helper (Service Bus/Dapr)

.NET Aspire service registration

Microsoft Agent Framework integration (state/memory)

Telemetry via OpenTelemetry

Agent example (FactCheckAgent):

```csharp
public sealed class FactCheckAgent : AgentBase
{
    protected override async Task HandleAsync(MessageEnvelope env, CancellationToken ct)
    {
        var req = env.Payload as FactCheckRequest ?? throw new ArgumentException(nameof(env.Payload));
        var result = await VerifyAsync(req, ct);
        await PublishAsync("factcheck.results", new FactCheckResult(req.StoryId, result.Pass, result.Flags, result.ReportMarkdown));
    }
}
```

Prompt templates (in /prompts) for each agent:

reporter_agent.md

factcheck_agent.md

copyedit_agent.md

seo_agent.md

distribution_agent.md

---

Editor Portal (Blazor)

Features:

Dashboard: List of active stories, their current stage, and SLA timers.

Pitch Board: Form for EIC to submit or approve pitches (headline, angle, embargo, keywords).

Story Pipeline View: Swimlane of stories by stage (Pitched → Published).

Policy Center: Configure paywall meter, human-approval thresholds, SEO rules, distribution channels.

Live View: Observe orchestration run for each story—logs, messages, agent statuses, manual override buttons.

Analytics: Overview of top stories by engagement, subscription conversions, churn.

Authentication: Microsoft Entra ID login; roles determine access and override capability.

---

CMS Abstraction & Publishing

(/libs/Newsroom.Cms)
Define:

```csharp
public interface ICmsPublisher
{
    Task<string> CreateOrUpdateAsync(Draft draft, PackagingResult pkg, CancellationToken ct);
    Task ScheduleAsync(string cmsId, DateTimeOffset when, CancellationToken ct);
    Task PublishNowAsync(string cmsId, CancellationToken ct);
}
```

Implement stub adapters:

WordPressVipPublisher

ArcXpPublisher

Or a simple MockPublisher for local/dev use.
PublishAgent uses ICmsPublisher to publish/schedule content.

---

Monetization & Paywall Logic

MonetizationAgent uses a rules engine (e.g., NRules or custom) to evaluate whether article goes behind paywall based on topic, user behaviour, subscription status.

Ad ops interface stubs for direct-sold sponsorship, native ad placements.

Subscription lifecycle: onboarding, trial conversion, win-back offers.

---

Testing & Quality

Unit tests: Test each agent’s logic and message routing.

Integration tests: Full end-to-end scenario (see below).

Load / performance tests: Located in /tests/load.

Linting/style: dotnet format, analyzers, SAST, dependency scanning (OSV).

---

Integration Test Scenario

Under /tests/integration/VeniceFestivalCoverageTest.cs, write a test that verifies the full pipeline when the Editor-in-Chief requests the story about 2025 Venice Film Festival:

Scenario Steps & Assertions:

EIC submits a pitch: “Coverage of the 2025 Venice International Film Festival (82nd edition), winners, notable premieres, sustainability trends in film.”

PitchAgent creates StoryPitch; AssignmentAgent assigns a reporter.

ReporterAgent produces a Draft containing:

Festival dates around late August/early September 2025 in Venice, Italy.

Major prize: Golden Lion; mention of a hypothetical winner.

A mention of sustainability in film or AI in cinema as trend.


FactCheckAgent passes the draft, verifying festival edition number, winner name etc.

CopyEditAgent produces headline variants; e.g., “Golden Lion awarded at 82nd Venice Film Festival” and SEO optimized deck.

SEOPackagingAgent produces metadata: keywords include “Venice Film Festival 2025”, “82nd Venice International Film Festival”, “Golden Lion”. Slug generated: venice-film-festival-2025-winners-premieres.

PublishAgent and DistributionAgent publish/schedule and distribute.

Assertions:

1. Final story status is Published.

2. Slug equals venice-film-festival-2025-winners-premieres.

3. Metadata keywords contain “Venice Film Festival 2025” and “Golden Lion”.

4. FeaturedImageUrl is non-null and matches expected pattern (e.g., https://assets.company.com/images/venice2025/…jpg).

5. Analytics event recorded for the StoryId with correct correlation.

6. No error states or manual override needed given the scenario.


Use xUnit or NUnit and a test harness simulating message bus or using in-memory pub/sub.

---

Deliverables

Fully working monorepo with solution and all projects.

IaC templates under /infra/.

Starter code for every service, base libraries, prompts and mocks.

Dockerfiles, GitHub Actions workflows.

Integration and unit test suite.

README.md with instructions for local dev (docker compose up), and Azure deploy (one-click via GitHub Actions).

Code uses .NET 8+ (nullable reference types enabled), C# 12, async/await, Microsoft Agent Framework, .NET Aspire conventions.

---

Coding Conventions

Use .NET 8 or later, C# 12, nullable enabled.

Strong typing; DI via Microsoft.Extensions.DependencyInjection.

Health checks and readiness endpoints.

Telemetry and correlation: each message and HTTP call carries CorrelationId (StoryId).

Resiliency: Use Polly policies for all external I/O (Service Bus, HTTP, OpenAI).

Secure defaults: secrets in Key Vault, no plaintext credentials.

Accessibility and performance for front end: SSR, lazy loading, Core Web Vitals.
