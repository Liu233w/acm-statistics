FROM mcr.microsoft.com/dotnet/sdk:5.0@sha256:7556a6a4194ebb21074022e90660410e277e6dae350c94fe42f3a7e2026db900 AS build

WORKDIR /src

COPY ["OHunt.Web/OHunt.Web.csproj", "OHunt.Web/"]
COPY ["OHunt.Tests/OHunt.Tests.csproj", "OHunt.Tests/"]

RUN dotnet restore "OHunt.Web/OHunt.Web.csproj"
RUN dotnet restore "OHunt.Tests/OHunt.Tests.csproj"

COPY . .
