FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster@sha256:5b59432f7579797a83fbceb87d3fb6a241d877a934f05be0526ced34a330c4f4 AS build

WORKDIR /src

COPY ["OHunt.Web/OHunt.Web.csproj", "OHunt.Web/"]
COPY ["OHunt.Tests/OHunt.Tests.csproj", "OHunt.Tests/"]

RUN dotnet restore "OHunt.Web/OHunt.Web.csproj"
RUN dotnet restore "OHunt.Tests/OHunt.Tests.csproj"

COPY . .
