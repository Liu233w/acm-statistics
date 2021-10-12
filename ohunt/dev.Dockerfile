FROM mcr.microsoft.com/dotnet/sdk:5.0@sha256:e7784454f02de388ba823d13ba5bcee9e1afa35c89672334f1b6dd2a10120a68 AS build

WORKDIR /src

COPY ["OHunt.Web/OHunt.Web.csproj", "OHunt.Web/"]
COPY ["OHunt.Tests/OHunt.Tests.csproj", "OHunt.Tests/"]

RUN dotnet restore "OHunt.Web/OHunt.Web.csproj"
RUN dotnet restore "OHunt.Tests/OHunt.Tests.csproj"

COPY . .
