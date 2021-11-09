FROM mcr.microsoft.com/dotnet/core/sdk:3.1

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

RUN dotnet restore ./PatchesAndAreasApi/PatchesAndAreasApi.csproj
RUN dotnet restore ./PatchesAndAreasApi.Tests/PatchesAndAreasApi.Tests.csproj

# Copy everything else and build
COPY . .

RUN dotnet build -c debug -o out PatchesAndAreasApi.Tests/PatchesAndAreasApi.Tests.csproj

CMD dotnet test
