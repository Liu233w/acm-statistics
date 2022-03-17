FROM mcr.microsoft.com/dotnet/sdk:6.0@sha256:9bae313dfa1699e935c963508917de903a63b055c3ddc9fc1b5955533e049b26

WORKDIR /src

COPY ["OHunt.Web/OHunt.Web.csproj", "OHunt.Web/"]
COPY ["OHunt.Tests/OHunt.Tests.csproj", "OHunt.Tests/"]

RUN dotnet restore "OHunt.Web/OHunt.Web.csproj"
RUN dotnet restore "OHunt.Tests/OHunt.Tests.csproj"

COPY . .
