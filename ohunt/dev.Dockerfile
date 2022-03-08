FROM mcr.microsoft.com/dotnet/sdk:6.0@sha256:16e355af1cfb7a82643fd450129c27641aeccd2beddac5f02d5c4d7755ef5bc2

WORKDIR /src

COPY ["OHunt.Web/OHunt.Web.csproj", "OHunt.Web/"]
COPY ["OHunt.Tests/OHunt.Tests.csproj", "OHunt.Tests/"]

RUN dotnet restore "OHunt.Web/OHunt.Web.csproj"
RUN dotnet restore "OHunt.Tests/OHunt.Tests.csproj"

COPY . .
