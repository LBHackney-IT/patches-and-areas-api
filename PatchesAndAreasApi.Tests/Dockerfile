FROM mcr.microsoft.com/dotnet/sdk:8.0-noble

# disable microsoft telematry
ENV DOTNET_CLI_TELEMETRY_OPTOUT='true'

ARG LBHPACKAGESTOKEN
ENV LBHPACKAGESTOKEN=$LBHPACKAGESTOKEN
WORKDIR /app

# Copy csproj and nuget config and restore as distinct layers
COPY ./PatchesAndAreasApi.sln ./
COPY ./PatchesAndAreasApi/PatchesAndAreasApi.csproj ./PatchesAndAreasApi/
COPY ./PatchesAndAreasApi.Tests/PatchesAndAreasApi.Tests.csproj ./PatchesAndAreasApi.Tests/
COPY /NuGet.Config /root/.nuget/NuGet/NuGet.Config
RUN dotnet restore

# Copy everything else and build
COPY . .

RUN dotnet build -c Release -o out PatchesAndAreasApi/PatchesAndAreasApi.csproj \
&& dotnet build -c debug -o out PatchesAndAreasApi.Tests/PatchesAndAreasApi.Tests.csproj

ENTRYPOINT ["dotnet", "test", "--collect", "XPlat Code Coverage;Format=opencover", "--results-directory", "./coverage"]
