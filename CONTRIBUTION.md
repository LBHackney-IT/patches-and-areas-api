# Contributing

## How to build and run

Install the .NET 10 SDK and Docker. Private Hackney packages come from GitHub Packages, so `LBHPACKAGESTOKEN` must be set. `NuGet.Config` reads that value.

Restore and build:

```sh
dotnet restore PatchesAndAreasApi.sln
dotnet build PatchesAndAreasApi.sln
```

Start DynamoDB Local and LocalStack:

```sh
docker compose up -d dynamodb-database localstack
```

DynamoDB Local listens on port 8000. LocalStack listens on port 4566. The `patches_and_areas_api` launch profile points the API at those addresses and serves `http://localhost:5000`. The health check is `http://localhost:5000/api/v1/healthcheck/ping`.

```sh
dotnet run --project PatchesAndAreasApi
```

`make serve` builds the API image and serves it on port 3000.

## Testing

Tests use xUnit, FluentAssertions, Moq, and BDDfy. Gateway and end-to-end tests talk to DynamoDB Local and LocalStack, so those containers need to be running first.

```sh
dotnet test PatchesAndAreasApi.Tests/PatchesAndAreasApi.Tests.csproj
```

`make test` builds the test image and runs the same suite in Docker. The image collects OpenCover coverage for the CircleCI SonarCloud step.

Gateway tests and the end-to-end stories are the `AppTest` collection. Use-case and controller tests do not need the databases.

## Development / workflow

Create a branch from `master` and open a pull request back into `master`. CircleCI checks formatting and runs the tests on that pull request.

A merge to `master` plans and applies the development Terraform, then deploys the Lambda to development. The `release` branch deploys staging, then production. Production Terraform and the production deploy each wait for a manual approval.

`check-code-formatting` installs `dotnet-format` and runs it with `--check`. Both projects treat warnings as errors, including the NetAnalyzers. The pull request template in `pull_request_template.md` is applied when a pull request is opened.
