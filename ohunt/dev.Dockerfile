FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster@sha256:e43cff5655ef77c5b1e9e64e5ace9c75c9071b9486b0e45e52ecb47f370e303c AS build

WORKDIR /src

COPY ["OHunt.Web/OHunt.Web.csproj", "OHunt.Web/"]
COPY ["OHunt.Tests/OHunt.Tests.csproj", "OHunt.Tests/"]

RUN dotnet restore "OHunt.Web/OHunt.Web.csproj"
RUN dotnet restore "OHunt.Tests/OHunt.Tests.csproj"

COPY . .
