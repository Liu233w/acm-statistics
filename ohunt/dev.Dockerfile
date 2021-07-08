FROM mcr.microsoft.com/dotnet/sdk:5.0@sha256:a682a67f577392f62eb4b3c650b1dc8861ba7988393a4c396f73b65a6b72d711 AS build

WORKDIR /src

COPY ["OHunt.Web/OHunt.Web.csproj", "OHunt.Web/"]
COPY ["OHunt.Tests/OHunt.Tests.csproj", "OHunt.Tests/"]

RUN dotnet restore "OHunt.Web/OHunt.Web.csproj"
RUN dotnet restore "OHunt.Tests/OHunt.Tests.csproj"

COPY . .
