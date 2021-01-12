FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster@sha256:7fd2afb637727e2278d03d587e8138e43191b536e1b9fa28c3e0ef2a199e44e6 AS build

WORKDIR /src

COPY ["OHunt.Web/OHunt.Web.csproj", "OHunt.Web/"]
COPY ["OHunt.Tests/OHunt.Tests.csproj", "OHunt.Tests/"]

RUN dotnet restore "OHunt.Web/OHunt.Web.csproj"
RUN dotnet restore "OHunt.Tests/OHunt.Tests.csproj"

COPY . .
