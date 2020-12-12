FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster@sha256:49156c8646f51c8e387d0cbe97082ba5a76d21c3e1b3149efdfe5690665be798 AS build

# sln 文件需要它
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
