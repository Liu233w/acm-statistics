FROM mcr.microsoft.com/dotnet/sdk:5.0@sha256:f89170534876d4adbbb7e4442285ae77242079f17b9dd8bd71d83551d491617f AS build

WORKDIR /src

COPY ["OHunt.Web/OHunt.Web.csproj", "OHunt.Web/"]
COPY ["OHunt.Tests/OHunt.Tests.csproj", "OHunt.Tests/"]

RUN dotnet restore "OHunt.Web/OHunt.Web.csproj"
RUN dotnet restore "OHunt.Tests/OHunt.Tests.csproj"

COPY . .
