FROM mcr.microsoft.com/dotnet/sdk:5.0@sha256:e6b7bbd8daa77b072c6f1e0c18f531e63381cc6c8d75ee922fa9fd504749966e AS build

WORKDIR /src

COPY ["OHunt.Web/OHunt.Web.csproj", "OHunt.Web/"]
COPY ["OHunt.Tests/OHunt.Tests.csproj", "OHunt.Tests/"]

RUN dotnet restore "OHunt.Web/OHunt.Web.csproj"
RUN dotnet restore "OHunt.Tests/OHunt.Tests.csproj"

COPY . .
