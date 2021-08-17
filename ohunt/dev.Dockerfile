FROM mcr.microsoft.com/dotnet/sdk:5.0@sha256:d1b7fa479a14b79c08b4d292fcd7289ac645220f9156c8ca58eaf4691be6ab4e AS build

WORKDIR /src

COPY ["OHunt.Web/OHunt.Web.csproj", "OHunt.Web/"]
COPY ["OHunt.Tests/OHunt.Tests.csproj", "OHunt.Tests/"]

RUN dotnet restore "OHunt.Web/OHunt.Web.csproj"
RUN dotnet restore "OHunt.Tests/OHunt.Tests.csproj"

COPY . .
