FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster@sha256:216b3953e4b497ed002868bc78bd2aef5f010a7a11980267b12a033db01db40d AS build

WORKDIR /src

COPY ["OHunt.Web/OHunt.Web.csproj", "OHunt.Web/"]
COPY ["OHunt.Tests/OHunt.Tests.csproj", "OHunt.Tests/"]

RUN dotnet restore "OHunt.Web/OHunt.Web.csproj"
RUN dotnet restore "OHunt.Tests/OHunt.Tests.csproj"

COPY . .
