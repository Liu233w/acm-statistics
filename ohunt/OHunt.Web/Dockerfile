#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim@sha256:e0c7ae65785a83a4e32e38ff0e2b1e2e3c4ad502e4ccfd83f4cd96374a38b3c4 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster@sha256:0fece15a102530aa2dad9d247bc0d05db6790917696377fc56a8465604ef1aff AS build
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
