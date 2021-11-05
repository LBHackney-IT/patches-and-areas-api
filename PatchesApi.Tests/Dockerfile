FROM mcr.microsoft.com/dotnet/core/sdk:3.1

# disable microsoft telematry
ENV DOTNET_CLI_TELEMETRY_OPTOUT='true'

ARG LBHPACKAGESTOKEN
ENV LBHPACKAGESTOKEN=$LBHPACKAGESTOKEN

WORKDIR /app

# Copy csproj and nuget config and restore as distinct layers
COPY ./PatchesApi.sln ./
COPY ./PatchesApi/PatchesApi.csproj ./PatchesApi/
COPY ./PatchesApi.Tests/PatchesApi.Tests.csproj ./PatchesApi.Tests/
COPY /NuGet.Config /root/.nuget/NuGet/NuGet.Config

RUN dotnet restore ./PatchesApi/PatchesApi.csproj
RUN dotnet restore ./PatchesApi.Tests/PatchesApi.Tests.csproj

# Copy everything else and build
COPY . .

RUN dotnet build -c debug -o out PatchesApi.Tests/PatchesApi.Tests.csproj

CMD dotnet test
