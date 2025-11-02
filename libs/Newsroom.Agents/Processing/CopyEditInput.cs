using Newsroom.Contracts;

namespace Newsroom.Agents.Processing;

public sealed record CopyEditInput(Draft Draft, FactCheckResult FactCheck);
