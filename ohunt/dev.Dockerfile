FROM mcr.microsoft.com/dotnet/sdk:5.0@sha256:de1fdb2c5b4d68a6b826614fae9eb6fc38bb025b0d1157aec97f156408342439 AS build

WORKDIR /src

COPY ["OHunt.Web/OHunt.Web.csproj", "OHunt.Web/"]
COPY ["OHunt.Tests/OHunt.Tests.csproj", "OHunt.Tests/"]

RUN dotnet restore "OHunt.Web/OHunt.Web.csproj"
RUN dotnet restore "OHunt.Tests/OHunt.Tests.csproj"

COPY . .
