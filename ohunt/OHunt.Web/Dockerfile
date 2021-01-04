#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim@sha256:fda96fad40e9fa3ebba78ba75bb6758752cfb9a955c279e7b3faad6dfe5ede51 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster@sha256:27df774804cbf186cd05d5d997078765aec2f78ca6f63499bd06a698e99ea46a AS build
WORKDIR /src
COPY ["OHunt.Web/OHunt.Web.csproj", "OHunt.Web/"]
RUN dotnet restore "OHunt.Web/OHunt.Web.csproj"
COPY . .
WORKDIR "/src/OHunt.Web"
RUN dotnet build "OHunt.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OHunt.Web.csproj" -c Release -o /app/publish

FROM base AS final
RUN apt-get update && apt-get install -y wait-for-it
ENV WAIT_COMMAND true
WORKDIR /app
COPY --from=publish /app/publish .
CMD $WAIT_COMMAND && dotnet OHunt.Web.dll
