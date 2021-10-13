FROM mcr.microsoft.com/dotnet/sdk:5.0@sha256:1bd86365c0de18fb256bfe44e615c693d6be82cedc74a9a7cfb86164ec9514e9 AS build

WORKDIR /src

COPY ["OHunt.Web/OHunt.Web.csproj", "OHunt.Web/"]
COPY ["OHunt.Tests/OHunt.Tests.csproj", "OHunt.Tests/"]

RUN dotnet restore "OHunt.Web/OHunt.Web.csproj"
RUN dotnet restore "OHunt.Tests/OHunt.Tests.csproj"

COPY . .
