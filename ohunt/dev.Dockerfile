FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster@sha256:2b0b6e9b4ceda402eebce563fb4eb155689dc989c5f154fc0b05efba1e11fd49 AS build

WORKDIR /src

COPY ["OHunt.Web/OHunt.Web.csproj", "OHunt.Web/"]
COPY ["OHunt.Tests/OHunt.Tests.csproj", "OHunt.Tests/"]

RUN dotnet restore "OHunt.Web/OHunt.Web.csproj"
RUN dotnet restore "OHunt.Tests/OHunt.Tests.csproj"

COPY . .
