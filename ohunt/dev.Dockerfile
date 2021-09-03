FROM mcr.microsoft.com/dotnet/sdk:5.0@sha256:d66207f54a4c1b6c8c4ff522237a00345a06d14154d9c143c6aa8b3e4f0e51bd AS build

WORKDIR /src

COPY ["OHunt.Web/OHunt.Web.csproj", "OHunt.Web/"]
COPY ["OHunt.Tests/OHunt.Tests.csproj", "OHunt.Tests/"]

RUN dotnet restore "OHunt.Web/OHunt.Web.csproj"
RUN dotnet restore "OHunt.Tests/OHunt.Tests.csproj"

COPY . .
