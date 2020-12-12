FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster@sha256:49156c8646f51c8e387d0cbe97082ba5a76d21c3e1b3149efdfe5690665be798 AS build

WORKDIR /src

COPY ["OHunt.Web/OHunt.Web.csproj", "OHunt.Web/"]
COPY ["OHunt.Tests/OHunt.Tests.csproj", "OHunt.Tests/"]

RUN dotnet restore "OHunt.Web/OHunt.Web.csproj"
RUN dotnet restore "OHunt.Tests/OHunt.Tests.csproj"

COPY . .
