#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0@sha256:a0a3af02a11b268c462fdb270abbcf8eee426d3c586bba40de5decdf898ffa67 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0@sha256:a5688a79685a3ff51f379615b781acada298558fa1ef032819a69b2dfead1c75 AS build
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
