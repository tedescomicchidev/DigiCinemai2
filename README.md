# DigiCinemai2 Newsroom Platform

This repository contains a .NET 8 mono-repo for the Azure-hosted multi-agent newsroom platform. It includes:

- Shared libraries for contracts, agent infrastructure, and CMS adapters.
- Individual agent services that can be containerised for Azure Container Apps or Azure Functions.
- An Aspire-friendly Blazor front end for the Editor-in-Chief experience.
- Bicep templates under `infra/` for core Azure resources.
- Integration tests demonstrating an end-to-end story pipeline for the 2025 Venice Film Festival scenario.

## Getting Started

1. Install the .NET 8 SDK.
2. Clone the repository and navigate to the root folder.
3. Restore and build the solution:
   ```bash
   dotnet restore DigiCinemai2.sln
   dotnet build DigiCinemai2.sln
   ```
4. Run tests (uses lightweight xUnit stubs when external feeds are unavailable):
   ```bash
   dotnet test DigiCinemai2.sln
   ```

## Repository Layout

- `apps/editor-portal` – Blazor web front end (server-side rendering).
- `apps/agents/*` – Worker services for each autonomous newsroom agent.
- `libs/Newsroom.Contracts` – Strongly-typed domain records, validators, and JSON schemas.
- `libs/Newsroom.Agents` – Agent hosting infrastructure, processors, and in-memory messaging helpers.
- `libs/Newsroom.Cms` – CMS abstraction and local `MockPublisher`.
- `prompts/` – Prompt templates for LLM-backed automation.
- `infra/` – Bicep templates for Azure resources.
- `tests/integration` – Integration pipeline test for Venice Film Festival coverage.

## Azure Deployment

The repository includes a starter GitHub Actions workflow (`.github/workflows/ci.yml`) that restores, builds, and tests the solution. Extend the workflow with publish/deploy steps once your Azure environment is provisioned via the provided Bicep template.

## Local Development Notes

- Agents currently use the in-memory message client for local runs; replace with Service Bus or Dapr implementations for distributed deployments.
- Validation is powered by a lightweight FluentValidation-compatible framework included in `Newsroom.Contracts` to avoid external dependencies in restricted environments.
- The integration test exercises the pitch-to-publication pipeline and can serve as a template for additional scenarios.
