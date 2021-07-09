FROM mcr.microsoft.com/dotnet/sdk:5.0@sha256:e8dce81012457ec0d87929aff8d60277c01b88fdd80bd128f1d4fc1c1bbdeb74 AS build

WORKDIR /src

COPY ["OHunt.Web/OHunt.Web.csproj", "OHunt.Web/"]
COPY ["OHunt.Tests/OHunt.Tests.csproj", "OHunt.Tests/"]

RUN dotnet restore "OHunt.Web/OHunt.Web.csproj"
RUN dotnet restore "OHunt.Tests/OHunt.Tests.csproj"

COPY . .
