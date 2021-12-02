FROM mcr.microsoft.com/dotnet/sdk:6.0@sha256:76dbd63a33da6510787e97c0943c4c4a2136936ce360cf9253862953e065035c

WORKDIR /src

COPY ["OHunt.Web/OHunt.Web.csproj", "OHunt.Web/"]
COPY ["OHunt.Tests/OHunt.Tests.csproj", "OHunt.Tests/"]

RUN dotnet restore "OHunt.Web/OHunt.Web.csproj"
RUN dotnet restore "OHunt.Tests/OHunt.Tests.csproj"

COPY . .
