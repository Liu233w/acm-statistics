FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster@sha256:45c253ab9e0bbbdbae3ba93a13eaa4a399ee030a65133d9a7ffbbbacaf0f1abf AS build

WORKDIR /src

COPY ["OHunt.Web/OHunt.Web.csproj", "OHunt.Web/"]
COPY ["OHunt.Tests/OHunt.Tests.csproj", "OHunt.Tests/"]

RUN dotnet restore "OHunt.Web/OHunt.Web.csproj"
RUN dotnet restore "OHunt.Tests/OHunt.Tests.csproj"

COPY . .
