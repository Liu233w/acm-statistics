FROM mcr.microsoft.com/dotnet/sdk:6.0@sha256:a220c3df8891dd6944afe28203273271f1947d3adb1652f7cc58e4edc1a8be0e

WORKDIR /src

COPY ["OHunt.Web/OHunt.Web.csproj", "OHunt.Web/"]
COPY ["OHunt.Tests/OHunt.Tests.csproj", "OHunt.Tests/"]

RUN dotnet restore "OHunt.Web/OHunt.Web.csproj"
RUN dotnet restore "OHunt.Tests/OHunt.Tests.csproj"

COPY . .
