#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0@sha256:5a8104c421dae626cbdc61c3426c8a8fc7e2fd9cd1c5eb54dbfa3962853b0c80 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0@sha256:8dcc61c6c6f42d4c38c07dadb58a78c6c2f729842846c39e9e978c1fb06f51b3 AS build
WORKDIR /src
COPY ["src/AcmStatisticsBackend.Web.Host/AcmStatisticsBackend.Web.Host.csproj", "src/AcmStatisticsBackend.Web.Host/"]
COPY ["src/AcmStatisticsBackend.Web.Core/AcmStatisticsBackend.Web.Core.csproj", "src/AcmStatisticsBackend.Web.Core/"]
COPY ["src/AcmStatisticsBackend.EntityFrameworkCore/AcmStatisticsBackend.EntityFrameworkCore.csproj", "src/AcmStatisticsBackend.EntityFrameworkCore/"]
COPY ["src/AcmStatisticsBackend.Core/AcmStatisticsBackend.Core.csproj", "src/AcmStatisticsBackend.Core/"]
COPY ["src/AcmStatisticsBackend.Application/AcmStatisticsBackend.Application.csproj", "src/AcmStatisticsBackend.Application/"]
RUN dotnet restore "src/AcmStatisticsBackend.Web.Host/AcmStatisticsBackend.Web.Host.csproj"
COPY . .
WORKDIR "/src/src/AcmStatisticsBackend.Web.Host"
RUN dotnet build "AcmStatisticsBackend.Web.Host.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AcmStatisticsBackend.Web.Host.csproj" -c Release -o /app/publish

FROM base AS final
RUN apt-get update && apt-get install -y wait-for-it
ENV WAIT_COMMAND true
WORKDIR /app
COPY --from=publish /app/publish .
CMD $WAIT_COMMAND && dotnet AcmStatisticsBackend.Web.Host.dll
