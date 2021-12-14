FROM mcr.microsoft.com/dotnet/sdk:6.0@sha256:4de6946e0473d465d238b4841b6180b3ef45559ea3c2d23f2b47e4bbeff54b7b

WORKDIR /src

COPY ["OHunt.Web/OHunt.Web.csproj", "OHunt.Web/"]
COPY ["OHunt.Tests/OHunt.Tests.csproj", "OHunt.Tests/"]

RUN dotnet restore "OHunt.Web/OHunt.Web.csproj"
RUN dotnet restore "OHunt.Tests/OHunt.Tests.csproj"

COPY . .
