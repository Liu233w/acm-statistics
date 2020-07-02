FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster@sha256:c87fd21bdebb2c2d573ecd703981476e5b8ac6a0e96d134722bb672e1c231fea AS build

WORKDIR /src

COPY ["OHunt.Web/OHunt.Web.csproj", "OHunt.Web/"]
COPY ["OHunt.Tests/OHunt.Tests.csproj", "OHunt.Tests/"]

RUN dotnet restore "OHunt.Web/OHunt.Web.csproj"
RUN dotnet restore "OHunt.Tests/OHunt.Tests.csproj"

COPY . .
