FROM mcr.microsoft.com/dotnet/sdk:5.0@sha256:d6946625eabb39346c063156113ffd9c7c23aaebe01534d4c7b1185436886a54 AS build

WORKDIR /src

COPY ["OHunt.Web/OHunt.Web.csproj", "OHunt.Web/"]
COPY ["OHunt.Tests/OHunt.Tests.csproj", "OHunt.Tests/"]

RUN dotnet restore "OHunt.Web/OHunt.Web.csproj"
RUN dotnet restore "OHunt.Tests/OHunt.Tests.csproj"

COPY . .
