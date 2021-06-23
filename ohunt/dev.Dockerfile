FROM mcr.microsoft.com/dotnet/sdk:5.0@sha256:ba0ee9dd1222981c5e01e616667a852aac85119631d3dcccb6d4ccd71a9a1163 AS build

WORKDIR /src

COPY ["OHunt.Web/OHunt.Web.csproj", "OHunt.Web/"]
COPY ["OHunt.Tests/OHunt.Tests.csproj", "OHunt.Tests/"]

RUN dotnet restore "OHunt.Web/OHunt.Web.csproj"
RUN dotnet restore "OHunt.Tests/OHunt.Tests.csproj"

COPY . .
