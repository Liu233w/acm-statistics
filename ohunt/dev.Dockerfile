FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster@sha256:1aa53e4fa32dbba836e64e7863955b6b2b165f3a4f8f8aeed648a249300fae07 AS build

WORKDIR /src

COPY ["OHunt.Web/OHunt.Web.csproj", "OHunt.Web/"]
COPY ["OHunt.Tests/OHunt.Tests.csproj", "OHunt.Tests/"]

RUN dotnet restore "OHunt.Web/OHunt.Web.csproj"
RUN dotnet restore "OHunt.Tests/OHunt.Tests.csproj"

COPY . .
