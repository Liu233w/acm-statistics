#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0@sha256:26ef9dc4aa354cc4aa4ae533c97f92d0d72c5e848f6968660be51d9fc1ce092e AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0@sha256:6187deae4799f812191b618d6d071dc30d1be692600b12af5046ff3b2c4b4658 AS build
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
