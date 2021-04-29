FROM mcr.microsoft.com/dotnet/sdk:6.0@sha256:48e3f1264a57fcde520784a5454fc0be18de1e7ef15f32d8db7879027aacba43 AS build

WORKDIR /src

COPY ["OHunt.Web/OHunt.Web.csproj", "OHunt.Web/"]
COPY ["OHunt.Tests/OHunt.Tests.csproj", "OHunt.Tests/"]

RUN dotnet restore "OHunt.Web/OHunt.Web.csproj"
RUN dotnet restore "OHunt.Tests/OHunt.Tests.csproj"

COPY . .
