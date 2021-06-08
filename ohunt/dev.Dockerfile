FROM mcr.microsoft.com/dotnet/sdk:5.0@sha256:4b582323b6ccf5197aef80d787c825be7831c9b7641ee43fe82bbb56140260c6 AS build

WORKDIR /src

COPY ["OHunt.Web/OHunt.Web.csproj", "OHunt.Web/"]
COPY ["OHunt.Tests/OHunt.Tests.csproj", "OHunt.Tests/"]

RUN dotnet restore "OHunt.Web/OHunt.Web.csproj"
RUN dotnet restore "OHunt.Tests/OHunt.Tests.csproj"

COPY . .
