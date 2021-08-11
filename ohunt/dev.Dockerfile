FROM mcr.microsoft.com/dotnet/sdk:5.0@sha256:a5688a79685a3ff51f379615b781acada298558fa1ef032819a69b2dfead1c75 AS build

WORKDIR /src

COPY ["OHunt.Web/OHunt.Web.csproj", "OHunt.Web/"]
COPY ["OHunt.Tests/OHunt.Tests.csproj", "OHunt.Tests/"]

RUN dotnet restore "OHunt.Web/OHunt.Web.csproj"
RUN dotnet restore "OHunt.Tests/OHunt.Tests.csproj"

COPY . .
