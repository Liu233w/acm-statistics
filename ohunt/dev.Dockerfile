FROM mcr.microsoft.com/dotnet/sdk:5.0@sha256:081a6c31153f16c02eeb7f1ad663adb8e1d21fa0d59402348c13bba7659a5419 AS build

WORKDIR /src

COPY ["OHunt.Web/OHunt.Web.csproj", "OHunt.Web/"]
COPY ["OHunt.Tests/OHunt.Tests.csproj", "OHunt.Tests/"]

RUN dotnet restore "OHunt.Web/OHunt.Web.csproj"
RUN dotnet restore "OHunt.Tests/OHunt.Tests.csproj"

COPY . .
