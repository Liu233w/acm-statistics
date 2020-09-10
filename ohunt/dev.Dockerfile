FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster@sha256:c31116825aebd79605b91dc032ce5049c0bb9454223ccfc70c9a07142592dbcb AS build

WORKDIR /src

COPY ["OHunt.Web/OHunt.Web.csproj", "OHunt.Web/"]
COPY ["OHunt.Tests/OHunt.Tests.csproj", "OHunt.Tests/"]

RUN dotnet restore "OHunt.Web/OHunt.Web.csproj"
RUN dotnet restore "OHunt.Tests/OHunt.Tests.csproj"

COPY . .
