FROM mcr.microsoft.com/dotnet/sdk:5.0@sha256:970a387bcf836c492ed631b3d9c9dbe484f9f32e7a2f9e9428dc357b0826da47 AS build

WORKDIR /src

COPY ["OHunt.Web/OHunt.Web.csproj", "OHunt.Web/"]
COPY ["OHunt.Tests/OHunt.Tests.csproj", "OHunt.Tests/"]

RUN dotnet restore "OHunt.Web/OHunt.Web.csproj"
RUN dotnet restore "OHunt.Tests/OHunt.Tests.csproj"

COPY . .
