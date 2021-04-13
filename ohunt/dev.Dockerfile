FROM mcr.microsoft.com/dotnet/sdk:5.0@sha256:85ea9832ae26c70618418cf7c699186776ad066d88770fd6fd1edea9b260379a AS build

WORKDIR /src

COPY ["OHunt.Web/OHunt.Web.csproj", "OHunt.Web/"]
COPY ["OHunt.Tests/OHunt.Tests.csproj", "OHunt.Tests/"]

RUN dotnet restore "OHunt.Web/OHunt.Web.csproj"
RUN dotnet restore "OHunt.Tests/OHunt.Tests.csproj"

COPY . .
