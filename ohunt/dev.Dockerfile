FROM mcr.microsoft.com/dotnet/sdk:5.0@sha256:60d3553b1f47a964f60173cb3fc6024a0a8b004abe482f38e01ea521c8fcf72e AS build

WORKDIR /src

COPY ["OHunt.Web/OHunt.Web.csproj", "OHunt.Web/"]
COPY ["OHunt.Tests/OHunt.Tests.csproj", "OHunt.Tests/"]

RUN dotnet restore "OHunt.Web/OHunt.Web.csproj"
RUN dotnet restore "OHunt.Tests/OHunt.Tests.csproj"

COPY . .
