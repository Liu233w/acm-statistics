FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster@sha256:bfd6083e9cd36b37b2a4e9f1722cc958b6654fa96cb3d84ef78492ecf00dcd32 AS build

WORKDIR /src

COPY ["OHunt.Web/OHunt.Web.csproj", "OHunt.Web/"]
COPY ["OHunt.Tests/OHunt.Tests.csproj", "OHunt.Tests/"]

RUN dotnet restore "OHunt.Web/OHunt.Web.csproj"
RUN dotnet restore "OHunt.Tests/OHunt.Tests.csproj"

COPY . .
