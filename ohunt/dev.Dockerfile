FROM mcr.microsoft.com/dotnet/sdk:5.0@sha256:b2f3f15ee6100efdd36819a429b75d936e4be71bb2487cc48223554f08e11285 AS build

WORKDIR /src

COPY ["OHunt.Web/OHunt.Web.csproj", "OHunt.Web/"]
COPY ["OHunt.Tests/OHunt.Tests.csproj", "OHunt.Tests/"]

RUN dotnet restore "OHunt.Web/OHunt.Web.csproj"
RUN dotnet restore "OHunt.Tests/OHunt.Tests.csproj"

COPY . .
