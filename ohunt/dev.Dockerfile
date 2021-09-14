FROM mcr.microsoft.com/dotnet/sdk:5.0@sha256:ffa15c10657d0bec7b1969fbd0b848fe5dd23c52de3819821e0e6ad02fa92df9 AS build

WORKDIR /src

COPY ["OHunt.Web/OHunt.Web.csproj", "OHunt.Web/"]
COPY ["OHunt.Tests/OHunt.Tests.csproj", "OHunt.Tests/"]

RUN dotnet restore "OHunt.Web/OHunt.Web.csproj"
RUN dotnet restore "OHunt.Tests/OHunt.Tests.csproj"

COPY . .
