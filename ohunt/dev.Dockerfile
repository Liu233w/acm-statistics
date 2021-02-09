FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster@sha256:1d5dd575dfebb6ed71d973b6a0be37f424306eda337a7dab09b6c0f53dffc2e3 AS build

WORKDIR /src

COPY ["OHunt.Web/OHunt.Web.csproj", "OHunt.Web/"]
COPY ["OHunt.Tests/OHunt.Tests.csproj", "OHunt.Tests/"]

RUN dotnet restore "OHunt.Web/OHunt.Web.csproj"
RUN dotnet restore "OHunt.Tests/OHunt.Tests.csproj"

COPY . .
