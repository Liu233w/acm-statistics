FROM mcr.microsoft.com/dotnet/sdk:6.0@sha256:fde93347d1cc74a03f1804f113ce85add00c6f0af15881181165ef04bc76bd00

WORKDIR /src

COPY ["OHunt.Web/OHunt.Web.csproj", "OHunt.Web/"]
COPY ["OHunt.Tests/OHunt.Tests.csproj", "OHunt.Tests/"]

RUN dotnet restore "OHunt.Web/OHunt.Web.csproj"
RUN dotnet restore "OHunt.Tests/OHunt.Tests.csproj"

COPY . .
