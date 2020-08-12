FROM mcr.microsoft.com/dotnet/core/sdk:5.0-buster@sha256:780f85d9f65dc05a524c09a5a341562a3e40be07a11c43fcca104b3ea0f05f33 AS build

WORKDIR /src

COPY ["OHunt.Web/OHunt.Web.csproj", "OHunt.Web/"]
COPY ["OHunt.Tests/OHunt.Tests.csproj", "OHunt.Tests/"]

RUN dotnet restore "OHunt.Web/OHunt.Web.csproj"
RUN dotnet restore "OHunt.Tests/OHunt.Tests.csproj"

COPY . .
