FROM mcr.microsoft.com/dotnet/sdk:5.0@sha256:7b443e16ee37213215027427420c43df4bf559bbc9f7425db4b2aee41657aef8 AS build

WORKDIR /src

COPY ["OHunt.Web/OHunt.Web.csproj", "OHunt.Web/"]
COPY ["OHunt.Tests/OHunt.Tests.csproj", "OHunt.Tests/"]

RUN dotnet restore "OHunt.Web/OHunt.Web.csproj"
RUN dotnet restore "OHunt.Tests/OHunt.Tests.csproj"

COPY . .
