FROM mcr.microsoft.com/dotnet/sdk:5.0@sha256:f1e629d176dcbe327ddbdbaa01862f362fb3b57c76d08ff925a88ffabec72f5e AS build

WORKDIR /src

COPY ["OHunt.Web/OHunt.Web.csproj", "OHunt.Web/"]
COPY ["OHunt.Tests/OHunt.Tests.csproj", "OHunt.Tests/"]

RUN dotnet restore "OHunt.Web/OHunt.Web.csproj"
RUN dotnet restore "OHunt.Tests/OHunt.Tests.csproj"

COPY . .
