FROM mcr.microsoft.com/dotnet/sdk:5.0@sha256:c05f1855774c8472961952f0b3ae5da61f3f959da77ea5f3318e39e780ccd5f3 AS build

WORKDIR /src

COPY ["OHunt.Web/OHunt.Web.csproj", "OHunt.Web/"]
COPY ["OHunt.Tests/OHunt.Tests.csproj", "OHunt.Tests/"]

RUN dotnet restore "OHunt.Web/OHunt.Web.csproj"
RUN dotnet restore "OHunt.Tests/OHunt.Tests.csproj"

COPY . .
