FROM mcr.microsoft.com/dotnet/sdk:5.0@sha256:ed69c36b90a31fb150d123df6a42245aceaf686b112b0034f39fb263f4e6917e AS build

WORKDIR /src

COPY ["OHunt.Web/OHunt.Web.csproj", "OHunt.Web/"]
COPY ["OHunt.Tests/OHunt.Tests.csproj", "OHunt.Tests/"]

RUN dotnet restore "OHunt.Web/OHunt.Web.csproj"
RUN dotnet restore "OHunt.Tests/OHunt.Tests.csproj"

COPY . .
