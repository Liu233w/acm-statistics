FROM mcr.microsoft.com/dotnet/sdk:6.0@sha256:16d0bf147784031d85040de10bd4b8c9dca2f634a7e19a85fd7bca8a63c987e1

# needed in sln file
RUN mkdir /build && echo '<?xml version="1.0" encoding="utf-8"?><Project ToolsVersion="15.0" Sdk="Microsoft.Docker.Sdk"></Project>' > /build/docker-compose.dcproj

WORKDIR /src

COPY ["src/AcmStatisticsBackend.Web.Host/AcmStatisticsBackend.Web.Host.csproj", "src/AcmStatisticsBackend.Web.Host/"]
COPY ["src/AcmStatisticsBackend.Web.Core/AcmStatisticsBackend.Web.Core.csproj", "src/AcmStatisticsBackend.Web.Core/"]
COPY ["src/AcmStatisticsBackend.EntityFrameworkCore/AcmStatisticsBackend.EntityFrameworkCore.csproj", "src/AcmStatisticsBackend.EntityFrameworkCore/"]
COPY ["src/AcmStatisticsBackend.Core/AcmStatisticsBackend.Core.csproj", "src/AcmStatisticsBackend.Core/"]
COPY ["src/AcmStatisticsBackend.Application/AcmStatisticsBackend.Application.csproj", "src/AcmStatisticsBackend.Application/"]
COPY ["test/AcmStatisticsBackend.Tests/AcmStatisticsBackend.Tests.csproj", "test/AcmStatisticsBackend.Tests/"]

RUN dotnet restore "src/AcmStatisticsBackend.Web.Host/AcmStatisticsBackend.Web.Host.csproj"
RUN dotnet restore "test/AcmStatisticsBackend.Tests/AcmStatisticsBackend.Tests.csproj"

COPY . .
