FROM mcr.microsoft.com/dotnet/sdk:5.0@sha256:ce2c0517c22301e48efb49dfe2a15a00dbf66d3fe286991eb70a7e337006469a AS build

WORKDIR /src

COPY ["OHunt.Web/OHunt.Web.csproj", "OHunt.Web/"]
COPY ["OHunt.Tests/OHunt.Tests.csproj", "OHunt.Tests/"]

RUN dotnet restore "OHunt.Web/OHunt.Web.csproj"
RUN dotnet restore "OHunt.Tests/OHunt.Tests.csproj"

COPY . .
