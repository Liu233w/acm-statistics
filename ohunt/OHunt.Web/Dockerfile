#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0@sha256:41faa82e297e4067a5d6c8803e4dbb64709880047f42ae6671820dd3bfff6ad6 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0@sha256:7b443e16ee37213215027427420c43df4bf559bbc9f7425db4b2aee41657aef8 AS build
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
