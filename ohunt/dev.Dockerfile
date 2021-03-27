FROM mcr.microsoft.com/dotnet/sdk:5.0@sha256:248d3d37927ca6aa606b41e2f94a17d90e0d3d9079573586b18961d791c399a4 AS build

WORKDIR /src

COPY ["OHunt.Web/OHunt.Web.csproj", "OHunt.Web/"]
COPY ["OHunt.Tests/OHunt.Tests.csproj", "OHunt.Tests/"]

RUN dotnet restore "OHunt.Web/OHunt.Web.csproj"
RUN dotnet restore "OHunt.Tests/OHunt.Tests.csproj"

COPY . .
