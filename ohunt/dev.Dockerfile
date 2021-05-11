FROM mcr.microsoft.com/dotnet/sdk:5.0@sha256:6c6bc8188aa055f97f8ab701adb6ecca46cffca4ccefe50fb531757cd6249738 AS build

WORKDIR /src

COPY ["OHunt.Web/OHunt.Web.csproj", "OHunt.Web/"]
COPY ["OHunt.Tests/OHunt.Tests.csproj", "OHunt.Tests/"]

RUN dotnet restore "OHunt.Web/OHunt.Web.csproj"
RUN dotnet restore "OHunt.Tests/OHunt.Tests.csproj"

COPY . .
