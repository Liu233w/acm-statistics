FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster@sha256:bbf31e5a084085e58743a4c34833c3678c685eba049ee17c13471bbb0e0b4796 AS build

WORKDIR /src

COPY ["OHunt.Web/OHunt.Web.csproj", "OHunt.Web/"]
COPY ["OHunt.Tests/OHunt.Tests.csproj", "OHunt.Tests/"]

RUN dotnet restore "OHunt.Web/OHunt.Web.csproj"
RUN dotnet restore "OHunt.Tests/OHunt.Tests.csproj"

COPY . .
