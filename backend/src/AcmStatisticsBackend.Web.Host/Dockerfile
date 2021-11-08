#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0@sha256:d8a410efaa2270e363212bfb23b6f739775f414529ee1a93264c4d5161208f06 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0@sha256:ed69c36b90a31fb150d123df6a42245aceaf686b112b0034f39fb263f4e6917e AS build
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
