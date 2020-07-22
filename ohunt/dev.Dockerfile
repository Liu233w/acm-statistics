FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster@sha256:7aa45680ecf79dd9288056511c08b2c40411869b9ab89a0b51aade1f83fcc12e AS build

WORKDIR /src

COPY ["OHunt.Web/OHunt.Web.csproj", "OHunt.Web/"]
COPY ["OHunt.Tests/OHunt.Tests.csproj", "OHunt.Tests/"]

RUN dotnet restore "OHunt.Web/OHunt.Web.csproj"
RUN dotnet restore "OHunt.Tests/OHunt.Tests.csproj"

COPY . .
