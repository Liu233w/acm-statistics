FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster@sha256:75774b7a31132e3bfdbbb362349c92e6df987856a61f666f523c307d2ae21850 AS build

WORKDIR /src

COPY ["OHunt.Web/OHunt.Web.csproj", "OHunt.Web/"]
COPY ["OHunt.Tests/OHunt.Tests.csproj", "OHunt.Tests/"]

RUN dotnet restore "OHunt.Web/OHunt.Web.csproj"
RUN dotnet restore "OHunt.Tests/OHunt.Tests.csproj"

COPY . .
