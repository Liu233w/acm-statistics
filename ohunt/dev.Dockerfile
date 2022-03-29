FROM mcr.microsoft.com/dotnet/sdk:6.0@sha256:d89f04400efcfe6d8dfbc2b9e24a662865ff5a2eda75f425d4a1a068e9a20b70

WORKDIR /src

COPY ["OHunt.Web/OHunt.Web.csproj", "OHunt.Web/"]
COPY ["OHunt.Tests/OHunt.Tests.csproj", "OHunt.Tests/"]

RUN dotnet restore "OHunt.Web/OHunt.Web.csproj"
RUN dotnet restore "OHunt.Tests/OHunt.Tests.csproj"

COPY . .
