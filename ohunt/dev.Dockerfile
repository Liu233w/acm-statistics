FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster@sha256:27df774804cbf186cd05d5d997078765aec2f78ca6f63499bd06a698e99ea46a AS build

WORKDIR /src

COPY ["OHunt.Web/OHunt.Web.csproj", "OHunt.Web/"]
COPY ["OHunt.Tests/OHunt.Tests.csproj", "OHunt.Tests/"]

RUN dotnet restore "OHunt.Web/OHunt.Web.csproj"
RUN dotnet restore "OHunt.Tests/OHunt.Tests.csproj"

COPY . .
