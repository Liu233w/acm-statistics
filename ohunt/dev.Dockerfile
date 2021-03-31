FROM mcr.microsoft.com/dotnet/sdk:5.0@sha256:48922a379d1f54390d5b8655933d3a62877c1fd46d44ab39cb4133fb1d230681 AS build

WORKDIR /src

COPY ["OHunt.Web/OHunt.Web.csproj", "OHunt.Web/"]
COPY ["OHunt.Tests/OHunt.Tests.csproj", "OHunt.Tests/"]

RUN dotnet restore "OHunt.Web/OHunt.Web.csproj"
RUN dotnet restore "OHunt.Tests/OHunt.Tests.csproj"

COPY . .
