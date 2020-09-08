FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster@sha256:6da604d96c720efca70be787f08fb92ab6a52c3b61a39a7d32cb6e79199408fb AS build

WORKDIR /src

COPY ["OHunt.Web/OHunt.Web.csproj", "OHunt.Web/"]
COPY ["OHunt.Tests/OHunt.Tests.csproj", "OHunt.Tests/"]

RUN dotnet restore "OHunt.Web/OHunt.Web.csproj"
RUN dotnet restore "OHunt.Tests/OHunt.Tests.csproj"

COPY . .
