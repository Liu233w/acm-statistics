FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster@sha256:43a9f3051f8b5e490020d53d746b957cf00df08bc4c182877bd7910f90323655 AS build

WORKDIR /src

COPY ["OHunt.Web/OHunt.Web.csproj", "OHunt.Web/"]
COPY ["OHunt.Tests/OHunt.Tests.csproj", "OHunt.Tests/"]

RUN dotnet restore "OHunt.Web/OHunt.Web.csproj"
RUN dotnet restore "OHunt.Tests/OHunt.Tests.csproj"

COPY . .
