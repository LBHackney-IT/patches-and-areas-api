FROM mcr.microsoft.com/dotnet/sdk:8.0

# disable microsoft telematry
ENV DOTNET_CLI_TELEMETRY_OPTOUT='true'

ARG LBHPACKAGESTOKEN
ENV LBHPACKAGESTOKEN=$LBHPACKAGESTOKEN

ARG SONAR_TOKEN
ENV SONAR_TOKEN=$SONAR_TOKEN


WORKDIR /app

# Install and run sonar cloud scanner
RUN apt-get update && apt-get install -y openjdk-17-jdk && apt-get clean \
&& dotnet tool install --global dotnet-sonarscanner
ENV PATH="$PATH:/root/.dotnet/tools"

RUN dotnet sonarscanner begin /k:"LBHackney-IT_patches-and-areas-api" \
/o:"lbhackney-it" /d:sonar.host.url=https://sonarcloud.io /d:sonar.login="${SONAR_TOKEN}"

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

CMD ["dotnet", "test"]
RUN dotnet sonarscanner end /d:sonar.login="${SONAR_TOKEN}"

