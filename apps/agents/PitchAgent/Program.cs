using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newsroom.Agents;
using Newsroom.Agents.Messaging;
using Newsroom.Agents.Processing;
using PitchAgent;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSingleton<IMessageClient, InMemoryMessageClient>();
builder.Services.AddSingleton<PitchAgentProcessor>();
builder.Services.AddAgentInfrastructure<PitchAgentWorker>();

var host = builder.Build();
host.Run();
